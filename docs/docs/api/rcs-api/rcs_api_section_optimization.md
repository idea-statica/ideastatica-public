# Optimizing an RCS Section

Before or after we have performed a calculation we may want to make some modifications to the reinforced cross-section which is assigned to a Section in the RCS Project through the API. 

We currently provide three ways to do this:

1. Changing the sections reinforced cross-section to another avaliable in the project.
2. Importing reinforcement and/or tendon modifications to an already avaliable reinforced cross-section in the project by a .nav file. 
3. Importing a new reinforced cross-section by .nav file import.

## Changing a Sections Reinforced Cross-section

If we have multiple reinforced cross-sections avaliable in the RCS Project we can switch between them.

# [.Net](#tab/linux)

[!code-csharp[](../../../../examples/api/csharp/rcs/Rcs-Api-Console-Tester/RcsConsoleApp/RcsCodeSamples.cs#changereinforcedcrosssection)]

# [Python](#tab/windows)

```python

    # First you may want to get avaliable reinforced cross-sections in project
    # Get IDs of all reinforced cross-sections in the rcs project
    rcsIds = []
    for rcs in rcsClient.Project.ReinforcedCrossSections.values():
        rcsIds.append(rcs.Id)
    
    sectionToUpdate = 1    

    # Set reinforced cross-section 2 to the section 1
    updateRes = rcsClient.UpdateReinforcedCrossSectionInSection(sectionToUpdate, rcsIds[1])

```

---

> [!NOTE]
> In IOM we can define any number of avaliable reinforced cross-sections that will be created in the RCS project. We can then use the API to switch between.

## Updating a reinforced cross-section

We may only want to update the reinforcement or tendon layout of an existing reinforced cross-section. We can  currently do this using the import reinforced cross-section functionality which is avaliible through the use of [.nav files](https://www.ideastatica.com/support-center/import-export-cross-section-reinforcement-and-tendons-in-rcs).

This allows us to

* Change only the reinforcement or tendon configuration of an existing reinforced cross-section.
* Change the cross-section (and keep reinforcement as is). 
* Completly overwrite the current reinforced cross-section with a different one from a template file.

Choose between the import options avaliable 'Reinf', 'Css' or 'Tendon' or 'Complete' we can detimine what will be imported.

# [.Net](#tab/linux)

[!code-csharp[](../../../../examples/api/csharp/rcs/Rcs-Api-Console-Tester/RcsConsoleApp/RcsCodeSamples.cs#changereinforcedcsslayout)]

# [Python](#tab/windows)

    #read a rcs template from NAV file
    rcs_template_file_path = os.path.join(dir_path, 'templates', 'rect-L-3-2.nav')
    print(rcs_template_file_path)

    reinfCssTemplate = None
    with open(rcs_template_file_path, 'r') as file:
        reinfCssTemplate = file.read()

    # The reinforcement of the provided Cross-section Id will be replaced with that provided in the .nav file 
    importSetting = rcsproject.ReinforcedCrossSectionImportSetting(1, "Reinf")

    newReinSect = rcsClient.ImportReinforcedCrossSection(importSetting, reinfCssTemplate)

---

## Adding a new reinforced cross-section

If you already have a fully defined reinforced cross-section within a .nav file, it can be imported as a new reinforced cross-section in the project. Remember if the Id provided is already used in the project it will ovveride it instead of creating a new one.

After import you can again change the reinforced cross-section of the required section to the one that has just been created.

# [.Net](#tab/linux)

[!code-csharp[](../../../../examples/api/csharp/rcs/Rcs-Api-Console-Tester/RcsConsoleApp/RcsCodeSamples.cs#addreinforcedcss)]

# [Python](#tab/windows)

```python

    #read a rcs template from NAV file
    rcs_template_file_path = os.path.join(dir_path, 'templates', 'rect-L-3-2.nav')
    print(rcs_template_file_path)

    reinfCssTemplate = None
    with open(rcs_template_file_path, 'r') as file:
        reinfCssTemplate = file.read()

    # if reinfCssId is None a new reinforced cross-section will be created 
    importSetting = rcsproject.ReinforcedCrossSectionImportSetting(None, "Complete")

    newReinSect = rcsClient.ImportReinforcedCrossSection(importSetting, reinfCssTemplate)
    print("Id of a new reinforced cross-section", newReinSect.Id)

```

---