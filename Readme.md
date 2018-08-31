In the Main method we are setting the filepaths to different files and calling helper methods sequentially to display the data
In the calculatewindowHour method we are calculating sliding window time and returning the value to main method
ReadstockValues is a generic method to read the actual and predicted values and we are calling this method twice to read both the files
CalculateError method is used to calculate the error between predicted and actual values
We are creating the output list by calling createoutput method and writing this value to a file in the main method