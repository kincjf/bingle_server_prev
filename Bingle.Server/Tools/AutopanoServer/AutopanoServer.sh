#!/bin/sh

#appname=`basename $0 | sed s,\.sh$,,`
#dirname=`dirname $0`
#LD_LIBRARY_PATH=$dirname

LD_LIBRARY_PATH=/usr/local/lib:`pwd`
export LD_LIBRARY_PATH
export

#export QT_PLUGIN_PATH="$dirname"
#$dirname/$appname "$@" &