from random import randrange
import os
import csv
import pickle
import numpy as np
import pandas as pd
import sys

from keras.layers import Input
from keras.models import load_model, Model

from keras.preprocessing.sequence import pad_sequences
from sklearn.manifold import TSNE


from sklearn.metrics import classification_report
from sklearn.metrics import confusion_matrix


LOSS_FUNCTION = 'binary_crossentropy'
OPTIMIZER = 'adamax'

MAX_LEN = 200
EMBEDDING_DIM = 100
BATCH_SIZE = 32

working_dir = './'

model_saved_path = working_dir

model_name = 'model.h5'

model = load_model(model_saved_path + model_name)

model.compile(loss= LOSS_FUNCTION,
              optimizer=OPTIMIZER,
              metrics=['accuracy'])

def LoadSavedData(path):
    with open(path, 'rb') as f:
        loaded_data = pickle.load(f)
    return loaded_data

def ListToCSV(list_to_csv, path):
    df = pd.DataFrame(list_to_csv)
    df.to_csv(path, index=False)

pickle_folder_path = sys.argv[1]
test_set_x = LoadSavedData(pickle_folder_path + 'test_set_x.pickle')
test_set_y = LoadSavedData(pickle_folder_path + 'test_set_y.pickle')

probs = model.predict(test_set_x, batch_size = BATCH_SIZE, verbose=1)

probability_folder_path = sys.argv[1]

ListToCSV(probs.tolist(), probability_folder_path + 'prob_assembly.csv')

'''
predicted_classes = []

for item in probs:
    if item[0] > 0.5:
        predicted_classes.append(1)
    else:
        predicted_classes.append(0)

ListToCSV(predicted_classes, 'classes_assembly.csv')

test_accuracy = np.mean(np.equal(test_set_y, predicted_classes))

test_set_y = np.asarray(test_set_y)

print ("LSTM classification result: ")

target_names = ["Non-vulnerable","Vulnerable"] #non-vulnerable->0, vulnerable->1
print (confusion_matrix(test_set_y, predicted_classes, labels=[0,1]))
print ("\r\n")
print ("\r\n")
print (classification_report(test_set_y, predicted_classes, target_names=target_names))'''
