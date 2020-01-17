from DatabaseHandler import DatabaseHandler

class JobHandler(object):
    def __init__(self, job, dbhandler: DatabaseHandler, config ):
        super().__init__()
        self.dbhandler = dbhandler 
        self.config = config
        self.job = job
    def run(self, job):
        pass