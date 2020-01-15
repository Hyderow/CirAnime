from enum import IntEnum


class JobStatus(IntEnum):
    Pending = 0
    InProgress = 1
    Finished = 2
    Failed = 3