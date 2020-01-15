class JobHandler(object):
    def __init__(self, dbhandler, config):
        super().__init__()
        self.dbhandler = dbhandler 
        self.config = config
    def run(self, job):
        pass