import sqlite3
import datetime
import urllib.parse
class DatabaseHandler(object):

    selectJobQueryString = "SELECT * FROM ProcessingJob WHERE Status = 0 ORDER BY CreationDate ASC;"
    UpdateJobStatusQueryString = "UPDATE ProcessingJob SET Status = ? WHERE ID = ?;"
    UpdateJobProgressQueryString = "UPDATE ProcessingJob SET Progress = ? WHERE ID = ?;"
    SelectMediaInfoQuery = "SELECT * FROM MediaInfo WHERE ID = ?"
    CreateNewJobQuery = "INSERT INTO ProcessingJob ([ID],[OriginalFile],[UploadEntryID],[Type],[Status],[CreationDate],[Progress],[Quality]) VALUES (?,?,?,?,?,?,?,?) "
    SelectUploadEntryQuery = "SELECT * FROM UploadEntry WHERE ID = ?"
    CreateSourceQuery = "INSERT INTO Source VALUES (?,?,?,?,?,?)"
    selectHighestSourceID = "SELECT ID FROM Source ORDER BY ID DESC LIMIT 1;"


    def __init__(self, dbpath):
        super().__init__()
        self.connection = sqlite3.connect(dbpath)
        self.connection.row_factory = sqlite3.Row

    def getNextJob(self):
        c = self.connection.cursor()
        c.execute(self.selectJobQueryString)
        job = None
        res = c.fetchone()
        if res != None:
            job = dict(res)
        return job
    def createNewJob(self, uploadID, origFile, jobtype, status, quality):
        c = self.connection.cursor()
        arguments = (None, origFile, uploadID, jobtype, status, datetime.datetime.utcnow(), 0, quality)
        
        c.execute(self.CreateNewJobQuery, arguments)
        self.connection.commit()

    def CreateNewSource(self, job, filename, quality):
        c = self.connection.cursor()
        c.execute(self.SelectUploadEntryQuery, (job["UploadEntryID"], ))
        uploadInfo = dict(c.fetchone())

        mediainfoID = uploadInfo["MediaInfoID"]
        escaped_filename = urllib.parse.quote(filename)

        url="https://ciranime.nyc3.digitaloceanspaces.com/" + escaped_filename
        contentType = "video/mp4"
        bitrate = 10000
        c.execute(self.CreateSourceQuery, (None, url, contentType, quality, bitrate, mediainfoID))
        self.connection.commit()


    def UpdateJobStatus(self, job, status):
        c = self.connection.cursor()
        args = (status, job["ID"])
        c.execute(self.UpdateJobStatusQueryString, args)
        self.connection.commit()

    def updateJobProgress(self, job, progress):
        c = self.connection.cursor()
        args = (progress, job["ID"])
        c.execute(self.UpdateJobProgressQueryString, args)
        self.connection.commit()

    def getNextSourceID(self):
        c = self.connection.cursor()
        c.execute(self.selectHighestSourceID)
        res = c.fetchone()
        if res is None:
            return 1
        return dict(res)["ID"] + 1
