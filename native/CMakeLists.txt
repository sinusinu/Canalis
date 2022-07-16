cmake_minimum_required(VERSION 3.13)
project(canalis VERSION 0.0.1)

add_library(${PROJECT_NAME} SHARED src/canalis.c)

set_target_properties(${PROJECT_NAME} PROPERTIES PREFIX "" OUTPUT_NAME "canalis-native" SUFFIX ".dll")