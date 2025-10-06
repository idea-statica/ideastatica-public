import os
from project_calculator_v2 import get_section_details
from project_calculator_v2 import calc_rcs_crack_width

dir_path = os.path.dirname(os.path.realpath(__file__))
rcs_project_file_path = os.path.join(dir_path, 'crack-width-example.IdeaRcs')

get_section_details(rcs_project_file_path)

#The increment of moment increase after each step.
moment_step = 3000

section_in_rcs_project = 1
calc_rcs_crack_width(rcs_project_file_path, section_in_rcs_project, moment_step)

