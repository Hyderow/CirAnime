import shutil
from JobHandler import JobHandler
import ffmpeg
import os
from JobStatus import JobStatus
from JobType import JobType

class PreprocessingHandler(JobHandler):
    def __init__(self, dbhandler, config):
        super().__init__(dbhandler, config)

    def run(self, job):
        infile = os.path.join(self.config["UnprocessedFilePath"], job["OriginalFile"])
        try:
            probe = ffmpeg.probe(infile)
        except ffmpeg.Error as err:
            errorOutput = err.stderr
            #self.dbhandler.setJobFailed(job,erroroutput)
        videostream = next((stream for stream in probe['streams'] if stream['codec_type'] == 'video'), None)
        height = videostream["coded_height"]
        (root, ext) = os.path.splitext(job["OriginalFile"])
        if (height >= 1080):
            newFilename = root + " [1080p]" + ext
        elif (height >= 720):
            newFilename = root + " [720p]" + ext
        else:
            newFilename = root + " [480p]" + ext

        if "mp4" in probe["format"]["format_name"]:
            print("no processing needed")
            self.copyFileToDestFolder(infile, newFilename)
            self.dbhandler.CreateNewSource(job, newFilename, height )
            if height >= 720:
                self.dbhandler.createNewJob(job["UploadEntryID"], infile, JobType.Encoding, JobStatus.Pending)
            #create new job, mark this one finished
        else:
            self.dbhandler.createNewJob(job["UploadEntryID"], infile, JobType.Encoding, JobStatus.Pending)
            #create new job with same
        self.dbhandler.UpdateJobStatus(job, JobStatus.Finished)


        
    def copyFileToDestFolder(self, filename, newfilename):
        srcpath = os.path.join(self.config["UnprocessedFilePath"], filename)
        dstpath = os.path.join(self.config["ProcessedFilePath"], newfilename)
        shutil.copyfile(srcpath, dstpath)