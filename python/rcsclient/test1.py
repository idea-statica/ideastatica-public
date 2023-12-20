import idea_statica_setup
import ideastatica_rcs_client

ideaStatiCa_Version = r'23.1'

ideaSetupDir = idea_statica_setup.get_ideasetup_path(ideaStatiCa_Version)

print(ideaSetupDir)

freeTcp = idea_statica_setup.get_free_port()
print(freeTcp)

rcsClient = ideastatica_rcs_client.ideastatica_rcs_client(ideaSetupDir, freeTcp)
try:
    print(rcsClient.printServiceDetails())

    # try to open an project
    projectId = rcsClient.OpenProject(r'C:\\x\\rcs-optim\\ToOptim.IdeaRcs')
    print(f'open project id = {projectId}')

    summary = rcsClient.ProjectSummary();
    print(summary)

except Exception as e:
    message  = str(e)
    print(f"An error occurred: {message}")
    exit(1)

finally:
    if not rcsClient is None:
        del rcsClient


