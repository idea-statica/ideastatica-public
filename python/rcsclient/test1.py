import idea_statica_setup
import ideastatica_rcs_client
import os

ideaStatiCa_Version = r'23.1'

ideaSetupDir = idea_statica_setup.get_ideasetup_path(ideaStatiCa_Version)

print(ideaSetupDir)

freeTcp = idea_statica_setup.get_free_port()
print(freeTcp)

rcsClient = ideastatica_rcs_client.ideastatica_rcs_client(ideaSetupDir, freeTcp)
try:
    print(rcsClient.printServiceDetails())

    # try to open an project

    dir_path = os.path.dirname(os.path.realpath(__file__))
    rcs_project_file_path = os.path.join(dir_path, 'projects', 'Project1.IdeaRcs')
    print(rcs_project_file_path)

    projectId = rcsClient.OpenProject(rcs_project_file_path)
    # Calculate all sections
    # sections = rcsClient.Project.Sections['RcsSectionModel']
    # for sect in sections:
    #     print(sect['Id'])

except Exception as e:
    message  = str(e)
    print(f"An error occurred: {message}")
    exit(1)

finally:
    if not rcsClient is None:
        del rcsClient


