import shutil
from JobHandler import JobHandler
import ffmpeg
import os
from JobStatus import JobStatus
from JobType import JobType
import re
from DatabaseHandler import DatabaseHandler
import math

class EncodingHandler(JobHandler):
    def __init__(self, job:dict, dbhandler: DatabaseHandler, config):
        super().__init__(job, dbhandler, config)

    def run(self):
        infile = os.path.join(self.config["UnprocessedFilePath"], self.job["OriginalFile"])
        probe = None
        try:
            probe = ffmpeg.probe(infile)
        except ffmpeg.Error as err:
            errorOutput = err.stderr
            print(errorOutput)
            return
            #self.dbhandler.setJobFailed(self.job,erroroutput)
        videostream = next((stream for stream in probe['streams'] if stream['codec_type'] == 'video'), None)
        subtitlestream = next((stream for stream in probe['streams'] if stream['codec_type'] == 'subtitle'), None)
        burnInSubs = False
        if subtitlestream != None:
            burnInSubs = True

        height = videostream["coded_height"]
        if "NUMBER_OF_FRAMES" in videostream["tags"].keys():
            self.number_of_frames = int(videostream["tags"]["NUMBER_OF_FRAMES"])
        elif "nb_frames" in videostream.keys():
            self.number_of_frames = int(videostream["nb_frames"])
        else:
            self.number_of_frames = 0
        isMP4 = "mp4" in probe["format"]["format_name"]
        # encodingHeight = self.determineResolution(height, isMP4)
        encodingHeight = self.job["Quality"]
        crf_value = 28 if isMP4 else 25
        (root, ext) = os.path.splitext(self.job["OriginalFile"])
        newFilename = root.replace("[1080p]", "").replace("[720p]", "").replace("[480p]", "") + "["+str(encodingHeight)+"p]" + ".mp4"

        encode_outfile = self.config["UnprocessedFilePath"] + newFilename
        self.encodeFile(infile, encodingHeight, encode_outfile, crf_value, burnInSubs) #TODO: Implement

        sourceid = self.dbhandler.getNextSourceID()
        encodedFilename = str(sourceid)+ "-" + newFilename

        deleteOriginalFile = encodingHeight == 480
        self.copyFileToDestFolder(newFilename, encodedFilename, deleteOriginalFile)
        self.removeFile(infile)


        self.dbhandler.CreateNewSource(self.job, encodedFilename, self.job["Quality"] )
        if encodingHeight >= 720:
            self.dbhandler.createNewJob(self.job["UploadEntryID"], newFilename, JobType.Encoding, JobStatus.Pending, 480)
        #create new job, mark this one finished
            #create new job with same
        self.dbhandler.UpdateJobStatus(self.job, JobStatus.Finished)
        
    def copyFileToDestFolder(self, filename, newfilename, removeOldFileOnSuccess=False):
        srcpath = os.path.join(self.config["UnprocessedFilePath"], filename)
        dstpath = os.path.join(self.config["ProcessedFilePath"], newfilename)
        shutil.copyfile(srcpath, dstpath)
        if removeOldFileOnSuccess:
            os.remove(srcpath)

    def determineResolution(self, height, isAlreadyMP4):
        if isAlreadyMP4:
            if height >= 1080:
                return 720
            elif height >= 720:
                return 480
        else:
            if height >= 1080:
                return 1080
            elif height >= 720:
                return 720
            else:
                return 480

    def encodeFile(self, infile, height, outfile, crf_value, burnInSubs):
        subs = None
        if burnInSubs:
            inputfile = ffmpeg.input(infile)
            audio = inputfile["a"]
            video = inputfile.filter("subtitles", infile)
            output = ffmpeg.output(audio, video, outfile, vcodec="h264", crf=crf_value, preset="ultrafast", s="hd"+str(height), tune="animation")
        else:        
            output = ffmpeg.input(infile).output(outfile, acodec="copy", vcodec="h264", crf=crf_value, preset="ultrafast", s="hd"+str(height), tune="animation")
        with output.overwrite_output().run_async(pipe_stdout=True, pipe_stderr=True) as process:
            print(process.args)
            for line in process.stderr:
                print(line, end='')
                self.processOutput(line)

    def processOutput(self, outputline):
        line = outputline.decode("utf-8")
        if not line.startswith("frame"):
            return
        m = re.search('frame= *([0-9]+).*$', line)
        if m != None:
            frames = int(m.group(1))
            if self.number_of_frames == 0:
                progress = frames
            else:
                progress = math.floor( frames / self.number_of_frames * 100)
            self.dbhandler.updateJobProgress(self.job, progress)

    def removeFile(self, file):
        os.remove(file)
