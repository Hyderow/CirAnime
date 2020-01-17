import shutil
from JobHandler import JobHandler
import ffmpeg
import os
from JobStatus import JobStatus
from JobType import JobType

class PreprocessingHandler(JobHandler):
    def __init__(self, job, dbhandler, config):
        super().__init__(job, dbhandler, config)

    def run(self):
        infile = os.path.join(self.config["UnprocessedFilePath"], self.job["OriginalFile"])
        try:
            probe = ffmpeg.probe(infile)
        except ffmpeg.Error as err:
            errorOutput = err.stderr
            #self.dbhandler.setJobFailed(self.job,erroroutput)
        videostream = next((stream for stream in probe['streams'] if stream['codec_type'] == 'video'), None)
        height = videostream["coded_height"]
        (root, ext) = os.path.splitext(self.job["OriginalFile"])
        if (height >= 1080):
            newFilename = root + " [1080p]" + ext
        elif (height >= 720):
            newFilename = root + " [720p]" + ext
        else:
            newFilename = root + " [480p]" + ext
        newFilename = newFilename.replace(" ", "_").replace("\"", "_").replace("\'", "_").replace("&", "_").replace("\\", "_")

        if "mp4" in probe["format"]["format_name"]:
            print("no processing needed")
            self.copyFileToDestFolder(infile, newFilename)
            self.dbhandler.CreateNewSource(self.job, newFilename, height )
            if height >= 720:
                self.dbhandler.createNewJob(self.job["UploadEntryID"], self.job["OriginalFile"], JobType.Encoding, JobStatus.Pending)
            #create new job, mark this one finished
        else:
            self.dbhandler.createNewJob(self.job["UploadEntryID"], self.job["OriginalFile"], JobType.Encoding, JobStatus.Pending)
            #create new job with same
        self.dbhandler.UpdateJobStatus(self.job, JobStatus.Finished)


        
    def copyFileToDestFolder(self, filename, newfilename, removeOldFileOnSuccess=False):
        srcpath = os.path.join(self.config["UnprocessedFilePath"], filename)
        dstpath = os.path.join(self.config["ProcessedFilePath"], newfilename)
        shutil.copyfile(srcpath, dstpath)
        if removeOldFileOnSuccess:
            shutil.rmtree(filename)