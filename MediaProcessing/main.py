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
import re
import urllib



selectJobQueryString = "SELECT * FROM ProcessingJob WHERE Status = 0 ORDER BY CreationDate ASC;"
UpdateJobStatusQueryString = "UPDATE ProcessingJob SET Status = ? WHERE ID = ?;"
SelectUploadEntryQuery = "SELECT * FROM UploadEntry WHERE ID = ?"

def main():
    # m = re.search('frame=([0-9]+).*$', 'fframe=37140 fps=206 q=24.0 size=   98048kB time=00:24:45.72 bitrate= 540.6kbits/s speed=8.25x')
    # print(m.group(0))
    # print(m.group(1))
    # infile = "in.mp4"
    # newfilename = "out2.mp4"
    # with ffmpeg.input(infile).output(newfilename, acodec="copy", vcodec="h264", crf=28, preset="ultrafast", s="hd480", tune="animation").overwrite_output().run_async(pipe_stdout=True, pipe_stderr=True) as process:
    #     for line in process.stderr:
    #         print(line, end='')

    # dbpath = "G:/Programming/CirAnime/CirAnime/CirAnime/ciranime.db"
    # inputPath = "file:H:/unprocessed/"
    
    with open("config.json", "r") as f:
        config = json.load(f)
    dbhandler = DatabaseHandler(dbpath)
    while True:
        job = dbhandler.getNextJob()

        if entry != None:
            try:
                job = dict(entry)
                if job["Type"] == JobType.PreProcessing:
                    PreprocessingHandler(job, dbhandler, config).run()
                elif job["Type"] == JobType.Encoding:
                    EncodingHandler(job, dbhandler, config).run()
            except Exception as ex:

        time.sleep(5)
        print("done")
      #  break

        pass



if __name__ == "__main__":
    main()
    print("after main")