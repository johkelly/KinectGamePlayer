KinectGamePlayer
================

Kinect -> Callback Function with raw skeleton data

Callback Function with raw skeleton data -> Raw Queue

Raw Queue -> Skeleton representation

Skeleton representation -(historgram library)-> histogrammed array

^John^ ----  vNathanv

histogrammed array -> libsvm predict ready data structure

libsvm predict ready data structure -(SVM.Predict)-> class

class -> Event trigger


Thread Diagram:
Parent: (Feeds each a shared memory address)
|       \
John    Nathan`
