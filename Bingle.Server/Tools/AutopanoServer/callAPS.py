#!/usr/bin/env python
# -*- coding: utf-8 -*-
# Copyright 2004-2012 - Kolor

import os, sys, argparse, types
from subprocess import *


def get_status_output(cmd, input=None, cwd=None, env=None):
    pipe =  Popen(
             cmd, cwd=cwd, env=env, stdout=PIPE, stderr=STDOUT
           )
    (output, errors) = pipe.communicate(input=input)
    assert not errors
    return pipe.returncode, output

class APS:
	def __init__(self, name):
		self.nameAPS = name
		self.pathAPS = "./"

class APS_args:
	def __init__(self):
		self.filenames = None
		self.folder = None
		self.panos = None
		self.temp_Folder = None
		self.output_Folder = None
		self.render_Filename = None
		self.project_Filename = None
		self.generic = None
		self.xml_list = None
		self.verbose = False
		self.log = True
		self.writePano = True
		self.noRender = True
		self.stitchAs = False

##
## APS python interface
##

def launchAPS( r ):
	MyAPS = APS("AutopanoServer_x64.exe")

	if (sys.platform == "linux2"):
		MyAPS.nameAPS = "AutopanoServer.sh"

	#Create absolute path to AutopanoServer/.sh/.exe  (same folder as callAPS.py)
	program = os.path.join(os.path.abspath(os.path.dirname(sys.argv[0])), MyAPS.nameAPS)

        # Setup command
	command = [program]

        # Add list of xml files to process in APG
	if r.xml_list != None:
            command.append("xml=%s" %r.xml_list)

        # Process callAPS options related to input datas
	if r.filenames != None:
		if r.verbose:
			print("===images=%s"%r.filenames)
		names = r.filenames.split(";")
		for file in names:
			if os.path.exists(file) == False:
				sys.exit("Error : File %s doesn't exist" %file)
		command.append("application/imagesSource=%s" %r.filenames)
	elif r.folder != None:
		if r.verbose:
			print("===folder=%s"%r.folder)
		if  os.path.exists(r.folder):
			command.append("application/inputFolder=%s" %r.folder)
		else:
			sys.exit("Error : Folder %s doesn't exist"%r.folder)
	elif r.panos != None:
		if r.verbose:
			print("===panos=%s"%r.panos)
		if  os.path.exists(r.panos):
			command.append("application/panoSource=%s"%r.panos)
		else:
			sys.exit("Error : Panorama %s doesn't exist" %r.panos)

        # Process callAPS options related to output
	if  r.output_Folder != None:
		print("Output folder: %s" %r.output_Folder)
		if  os.path.exists(r.output_Folder):
                    command.append("pano/render_folderTpl=%s" % r.output_Folder)
	if r.render_Filename != None:
            command.append("pano/render_filenameTpl=%s" % r.render_Filename)
        if r.project_Filename != None:
            command.append("pano/project_filenameTpl=%s" % r.project_Filename)

	# Process callAPS options related to temp folders
	if  r.temp_Folder != None:
		if  os.path.exists(r.temp_Folder):
			command.append("application/tempFolderPath=%s" %temp_Folder)

        # Process general callAPS options
	if r.log != True:
		command.append("application/Log=2")

	if r.writePano != True: # store_true est inverse
		command.append("pano/auto_Save=True")

	if r.noRender != True:
		command.append("pano/auto_Render=False")

	if r.stitchAs == True:
                command.append("stitchAs=1")

        # Process -gen option
	if r.generic != None:
		if isinstance(r.generic, types.StringTypes):
			command.append(r.generic)
		else:
			for gen in r.generic:
				command.append(gen)

	# Debug
	if r.verbose:
		print("\nProgram: "+program)
		print("\nCommand (callAPS.py):")
		print(command)

	(status, output) = get_status_output(command)
	if r.verbose:
		print("status: %d" % status)
		print("output: %s" % output)
		print("CallAPS: done")


if __name__ == "__main__":
	parser = argparse.ArgumentParser(prog = "CallAPS")

	# Set default values
	parser.set_defaults(verbose=True)

	# Mandatory Options
	group = parser.add_argument_group("mandatory options (mutually exclusive)")
	#group = parser.add_mutually_exclusive_group() #currently mutually exclusive argument groups do not support the title and description arguments of add_argument_group()
	group.add_argument("-i", dest="filenames",
				help="absolute path to a list of images to be stitched")
	group.add_argument("-f", dest="folder",
				help="absolute path to a folder which will be analysed")
	group.add_argument("-p", dest="panos",
				help="absolute path to .pano file to be rendered")

	# Folder options
	group = parser.add_argument_group("folder options")
	group.add_argument("-tmp", dest="temp_Folder",
				help="absolute path to temporary folder")
	group.add_argument("-o", dest="output_Folder",
				help="folder where resulting panoramas will be rendered")

	# Debug Options
	group = parser.add_argument_group("debug options")
	group.add_argument("-v", action="store_true",  dest="verbose", help="verbose mode")
	group.add_argument("-q", action="store_false",  dest="verbose", help="quiet mode")
	group.add_argument("-log", action="store_false", dest="log", help="export log")

	# Optional parameters
	group = parser.add_argument_group("optional parameters")
	#General options
	group.add_argument("-xml",  dest="xml_list",
				help="xml configuration files")
	group.add_argument("-r",  dest="render_Filename",
				help="template name for the rendered panorama")
	group.add_argument("-pf",  dest="project_Filename",
				help="template name for the .pano project file")
	group.add_argument("-gen",  dest="generic",
				help="generic command to use direct registry entry")
	group.add_argument("-w",  action="store_false", dest="writePano",
				help="write a .pano file")
	group.add_argument("-noRender",  action="store_false", dest="noRender",
				help="do not render panorama")
        group.add_argument("-stitchAs", action="store_true", dest="stitchAs",
                               help="Stitch the panorama as the given .pano by searching control points in overlapping areas")

	r = parser.parse_args()
	launchAPS(r)
