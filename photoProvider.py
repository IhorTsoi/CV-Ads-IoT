import cv2
import sys

pathToSaveThePhoto = sys.argv[1]

cam = cv2.VideoCapture(0)

result, image = cam.read()
if result:
	cv2.imwrite(pathToSaveThePhoto, image)
	sys.exit(0)
else:
	sys.exit(1)
