import os
from project_calculator_v2 import get_section_details
from project_calculator_v2 import calc_rcs_crack_width
from project_calculator_v2 import calc_rcs_proj_variants

dir_path = os.path.dirname(os.path.realpath(__file__))
rcs_project_file_path = os.path.join(dir_path, 'crack-width-example.IdeaRcs')

get_section_details(rcs_project_file_path)

#The increment of moment increase after each step.
moment_step = 3000

section_in_rcs_project = 1
calc_rcs_crack_width(rcs_project_file_path, section_in_rcs_project, moment_step)


# section to calulate
section_in_rcs_project = 1
reinforced_css_templates = ["rect-L-2-2.nav","rect-L-3-2.nav","rect-L-4-2.nav"]

# file name of a rsc project to be calculate
project_to_calculate = rcs_project_file_path = os.path.join(dir_path, 'Project2.IdeaRcs')

# calculate all variants 
df_sectionChecks = calc_rcs_proj_variants(project_to_calculate, section_in_rcs_project, reinforced_css_templates)

