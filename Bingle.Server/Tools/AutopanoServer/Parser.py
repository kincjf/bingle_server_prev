# -*- coding: utf-8 -*-
import os
import sys
import argparse
import datetime

from callAPS import *

program = "./CallAPS.py"

def processDirectory(path, preset_xml, output_path, verbose):
        config_xml = os.path.join(path,  'APS-config.xml')
	if os.path.exists(config_xml):
		if verbose:	
			print("\nFound APS-config.xml in subfolder: "+path)
		arguments = APS_args()
		arguments.folder = path
		arguments.xml_list = preset_xml+ ";" + config_xml
		output_Filename  = os.path.basename(path)
		arguments.render_Filename =  output_Filename
		arguments.project_Filename = output_Filename
		arguments.output_Folder = output_path
		if verbose:
			arguments.verbose = True
			print("\narguments (Parser.py):")
			print(arguments)
		launchAPS(arguments)
	else:
                list = os.listdir(path)
                for dir in list:
                        subpath = os.path.join(path,  dir)
                        if os.path.isdir(subpath) and os.path.exists(subpath):
                                if r.verbose:
                                        print("step folder to process: " + subpath)
                        else:
                                continue
                        processDirectory(subpath, preset_xml, output_path, verbose)
	
if __name__ == "__main__":
        if  os.path.exists(program) == False:
                sys.exit("Error: %s doesn't exist" %program)
                
        parser = argparse.ArgumentParser(prog = 'Parser')

        group = parser.add_argument_group("mandatory options (-bdd and -f are mutually exclusive)")
        group.add_argument("-bdd", dest="BDD",
                                help="absolute path to the data base")
        group.add_argument("-f", dest="folder",
                                help="absolute path to one specific folder")	
        group.add_argument("-xml", dest="preset",
                                help="absolute path to preset xml file")	
        group.add_argument("-o", dest="Output",
                                help="absolute path to the output")

        group2 = parser.add_argument_group("Other options")	
        group2.add_argument("-path", dest="pathPy",
                                help="path to CallAPS.py")
        group2.add_argument("-v", action="store_true",  default=False,  dest="verbose", help="verbose mode")

        r = parser.parse_args()

        if r.verbose:
                print("\nProgram: " + program)

        pathBDD = None

        if r.BDD != None:
                if os.path.exists(r.BDD):
                        pathBDD = r.BDD
                else:
                        sys.exit("Error (Parser.py l.50): folder %s doesn't exist" %r.BDD)
        elif r.folder != None:
                if os.path.exists(r.folder):
                        folder = r.folder
                else:
                        sys.exit("Error (Parser.py l.55): folder %s doesn't exist" %r.folder)
        else:		
                sys.exit("Error (Parser.py l.57): BDD path or specific folder is needed (-bdd or -folder)")

        if r.preset != None:
                if os.path.exists(r.preset):
                        preset_xml = r.preset
                else:
                        sys.exit("Error (Parser.py l.63): preset file %s doesn't exist" %r.preset)
        else:
                sys.exit("Error (Parser.py l.65): xml preset file absolute path is mandatory")
                        
        if r.Output != None:
                if os.path.exists(r.Output) == False:
                        print("Folder %s doesn't exist - trying to make dir" %r.Output)
                        os.mkdir(r.Output)
        else:
                sys.exit("Error (Parser.py l.72): output folder is needed")
                        
        if r.pathPy != None:
                if os.path.exists(r.pathPy):
                        program = r.pathPy
                else:
                        print("File %s doesn't exist - using default: %s" %r.pathPy %program)
                
        now = datetime.datetime.now()
        output_path = os.path.join(r.Output,  "AutopanoServer check "+now.strftime("%Y.%m.%d %H.%M"))
        if os.path.exists(output_path) == False:
                os.mkdir(output_path)
        elif r.verbose:
                print("Folder %s already exists" %output_path)

        if pathBDD != None:
                processDirectory(pathBDD, preset_xml, output_path, r.verbose)
                
        elif folder != "":
                config_xml = os.path.join(folder,  'APS-config.xml')
                if os.path.exists(config_xml):
                        arguments = APS_args()
                        arguments.folder = folder
                        arguments.xml_list = preset_xml+ ";" + config_xml
                        filename = "Specific folder test - " + os.path.basename(folder)
                        arguments.render_Filename =  filename
                        arguments.project_Filename = filename
                        arguments.output_Folder = output_path
                        if r.verbose:
                                arguments.verbose = True
                                print("\nlaunchAPS arguments (Parser.py):")
                                print(arguments)
                        launchAPS(arguments)	
                else:
                        sys.exit("Error (Parser.py l.133): config file %s doesn't exist" %config_xml)	
