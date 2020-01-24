import time
import sqlite3
import ffmpeg
import sys
import subprocess
from enum import Enum
from PreprocessingHandler import PreprocessingHandler
from EncodingHandler import EncodingHandler
from DatabaseHandler import DatabaseHandler
import json
from JobType import JobType
from JobStatus import JobStatus
import re
import urllib
import platform


selectJobQueryString = "SELECT * FROM ProcessingJob WHERE Status = 0 ORDER BY CreationDate ASC;"
UpdateJobStatusQueryString = "UPDATE ProcessingJob SET Status = ? WHERE ID = ?;"
SelectUploadEntryQuery = "SELECT * FROM UploadEntry WHERE ID = ?"

def main():
    # m = re.search('frame=([0-9]+).*$', 'fframe=37140 fps=206 q=24.0 size=   98048kB time=00:24:45.72 bitrate= 540.6kbits/s speed=8.25x')
    # print(m.group(0))
    # print(m.group(1))ss
    # infile = "in.mkv"
    # infile = "/mnt/h/unprocessed/YamanoSusume2ndSeason-01(BD720pAAC), [720p].mkv"
    # newfilename = "out2.mp4"

    

    # p = ffmpeg.probe(infile)
    
    # inputfile = ffmpeg.input(infile)
    # audio = inputfile["a"]
    # v0 = inputfile["v"]
    # s0 = inputfile["s"]
    # video = inputfile.filter("subtitles", infile)
    # output = ffmpeg.output(audio, video, newfilename,vcodec="h264", crf=36, preset="ultrafast", s="hd480", tune="animation")
    # print(output.get_args())
    # output.run()
    # return
    # ffmpeg.input(infile).overlay()
    # print(ffmpeg.overlay(v0,s0).output(newfilename).get_args())

    #outputfile = ffmpeg.output(newfilename)
    # with inputfile.filter_("subtitles", infile).output(newfilename, vcodec="h264", crf=36, preset="ultrafast", s="hd480", tune="animation").run_async() as p:
    #     print(p.args)
    #     p.readlines()
        # with ffmpeg.input(infile).output(newfilename, acodec="copy", vcodec="h264", crf=36, preset="ultrafast", s="hd480", tune="animation", vf="subtitles="+"\""+infile+"\"").overwrite_output().run_async(pipe_stdout=True, pipe_stderr=True) as process:
        # print(process.args)
        # for line in process.stderr:
            # print(line, end='')
    # return
    # dbpath = "G:/Programming/CirAnime/CirAnime/CirAnime/ciranime.db"
    # inputPath = "file:H:/unprocessed/"
    platformName = platform.system()
    # configName = platformName == "Windows" ? "config.json" : "configWSL.json"
    configName = "config.json"
    with open(configName, "r") as f:
        config = json.load(f)
    dbhandler = DatabaseHandler(config["DBPath"])

    while True:
        job = dbhandler.getNextJob()
        if job != None:
            try:
                if job["Type"] == JobType.PreProcessing:
                    PreprocessingHandler(job, dbhandler, config).run()
                elif job["Type"] == JobType.Encoding:
                    EncodingHandler(job, dbhandler, config).run()
            except Exception as ex:
                ex.__traceback__.print_tb()
            #     dbhandler.UpdateJobStatus(job, JobStatus.Failed)
                #TODO: clean up created files

        time.sleep(5)
        print("done")



if __name__ == "__main__":
    main()
    print("after main")