import time
import sqlite3
import ffmpeg
import sys
import subprocess
from enum import Enum
from PreprocessingHandler import PreprocessingHandler
from DatabaseHandler import DatabaseHandler
import json




selectJobQueryString = "SELECT * FROM ProcessingJob WHERE Status = 0 ORDER BY CreationDate ASC;"
UpdateJobStatusQueryString = "UPDATE ProcessingJob SET Status = ? WHERE ID = ?;"
SelectUploadEntryQuery = "SELECT * FROM UploadEntry WHERE ID = ?"

def main():
    dbpath = "G:/Programming/CirAnime/CirAnime/CirAnime/ciranime.db"
    inputPath = "file:H:/unprocessed/"
    
    with open("config.json", "r") as f:
        config = json.load(f)

    connection = sqlite3.connect(config["DBPath"])
    connection.row_factory=sqlite3.Row
    c = connection.cursor()


    dbhandler = DatabaseHandler(dbpath)
    while True:
        # getConversionJob()
        # handleJob()
        # moveFile()
        # modifyMediaInfo
        # create further conversion job
        c.execute(selectJobQueryString)
        job = dict(c.fetchone())
        
        if job["Type"] == 0:
            handler = PreprocessingHandler(dbhandler, config).run(job)
           

        id = (job["UploadEntryID"], )
        
        '''
        process = ffmpeg.input("video.mkv",  r=10).output("processed3.mp4", acodec="copy", vcodec="h264").run_async(pipe_stdout=True, pipe_stderr=True)
        try:
            out, err = process.communicate(timeout=10)
        except subprocess.TimeoutExpired:
            process.kill()
            out, err = process.communicate()
            print("out: #### ", out)
            print("err: ### ", err)
        '''
        time.sleep(10)
        print("done")
        break

        pass



if __name__ == "__main__":
    main()
    print("after main")