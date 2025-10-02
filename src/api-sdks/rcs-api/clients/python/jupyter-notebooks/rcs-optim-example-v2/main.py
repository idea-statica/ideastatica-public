import os
from project_calculator_v2 import get_section_details

dir_path = os.path.dirname(os.path.realpath(__file__))
rcs_project_file_path = os.path.join(dir_path, 'crack-width-example.IdeaRcs')

get_section_details(rcs_project_file_path)