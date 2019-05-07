import os
import pandas as pd
import csv
import pickle
import numpy as np
import datetime
import sys
from keras.preprocessing.sequence import pad_sequences

MAX_LEN = 200
EMBEDDING_DIM = 100
EPOCHS = 100
BATCH_SIZE = 32
PATIENCE = 40

working_dir = './'
w2v_model_path = working_dir + 'assemble_model_CBOW.txt'

binary_folder_path = working_dir
pickle_folder_path = "test_files/"

good_csv = binary_folder_path + 'binary_good.csv'

# 1. Load data.

def getData(file_path):

    # The encoding has to be 'latin1' to make sure the string to float convertion to be smooth
    # df = pd.read_csv(file_path, sep=',', encoding='latin1', low_memory=False, header=None, quoting=csv.QUOTE_NONE, error_bad_lines=False)
    # df_list = df.values.tolist()
    df_list = []
    CSV = csv.reader(open(file_path, 'r', encoding='latin1'))
    for row in CSV:
        df_list.append(row)
    print(len(df_list))
    temp = []
    for i in df_list:
        # Get rid of 'NaN' values.
        i = [x for x in i if str(x) != 'nan']
        i = [str(x) for x in i]
        new_i = []
        for item in i:
            sub_temp = item.split()
            for sub_item in sub_temp:
                new_i.append(sub_item)
        temp.append(new_i)

    return temp


def getID(file_path):

    # The encoding has to be 'latin1' to make sure the string to float convertion to be smooth
    df = pd.read_csv(file_path, sep=',', encoding='latin1', low_memory=False, header=None)
    df_list = df.values.tolist()

    return df_list

Assembly_test_list = getData(good_csv)
Assembly_test_label_list = [1] * len(Assembly_test_list)
Assembly_test_id_list = range(len(Assembly_test_list))

total_list = Assembly_test_list
total_list_label = Assembly_test_label_list
total_list_id = Assembly_test_id_list
#--------------------------------------------------------#

# 2. Tokenization: convert the loaded text to tokens.
def LoadSavedData(path):
    with open(path, 'rb') as f:
        loaded_data = pickle.load(f)
    return loaded_data

tokenizer = LoadSavedData(working_dir + 'assemble_tokenizer.pickle')

# ----------------------------------------------------- #
# 3. Train a Vocabulary with Word2Vec -- using the function provided by gensim

w2v_model = open(w2v_model_path)

# 4. Use the trained tokenizer to tokenize the sequence.
#-------------------------------------------------------------------
new_total_token_list = []

for sub_list_token in total_list:
    new_line = ','.join(sub_list_token)
    new_total_token_list.append(new_line)


total_sequences = tokenizer.texts_to_sequences(new_total_token_list)
word_index = tokenizer.word_index
print ('Found %s unique tokens.' % len(word_index))

print ("The length of tokenized sequence: " + str(len(total_sequences)))

#------------------------------------#
# 5. Do the paddings.
#--------------------------------------------------------

print ("max_len ", MAX_LEN)
print('Pad sequences (samples x time)')

total_sequences_pad = pad_sequences(total_sequences, maxlen = MAX_LEN, padding ='post')
#total_sequences_pad.reshape((2030,200))

print ("The shape after paddings: ")
print (total_sequences_pad.shape)



#train_set_x, validation_set_x, train_set_y, validation_set_y, train_set_id, validation_set_id = train_test_split(total_sequences_pad, total_list_label, total_list_id, test_size=0.2, random_state=42)
#
#test_set_x, validation_set_x, test_set_y, validation_set_y, test_set_id, validation_set_id = train_test_split(validation_set_x, validation_set_y, validation_set_id, test_size=0.5, random_state=42)
def ListToCSV(list_to_csv, path):
    df = pd.DataFrame(list_to_csv)
    df.to_csv(path, index=False)


# ListToCSV(test_set_x.tolist(), 'TTtest_set_x.csv')

# print('xinbo')
# print(len(test_set_x))


# Save the test data sets.
#--------------------------------------------------------------
with open(pickle_folder_path +  'test_set_x.pickle', 'wb') as handle:
    #pickle.dump(tokenizer, handle, protocol=pickle.HIGHEST_PROTOCOL)
    pickle.dump(total_sequences_pad, handle, protocol=2)

with open(pickle_folder_path +  'test_set_y.pickle', 'wb') as handle:
    #pickle.dump(tokenizer, handle, protocol=pickle.HIGHEST_PROTOCOL)
    pickle.dump(Assembly_test_label_list, handle, protocol=2)
