Below are the seen possible ways of interacting with Checkbot:

# Controlling Checkbot from a Third-Party Application

The most direct method for a user to interact with Checkbot is to allow execution of the Checkbot app from inside a third-party application. Typically, this is achieved by creating a plug-in application that can be installed as a part of the base application.

**Limitation to this type of interaction:**
* The base application must allow for plug-in Apps to be installed.
* Being able to retrieve a selection of elements, nodes and connections are generally required also.

## Before you Start

### Understand Application Interaction

In most cases, someone close to the development of the third-party application will be undertaking the task of creating the BIM Link however, it is important to understand how the application allows for interaction and how this may limit or change the way interaction with Checkbot is to occur:

* **Does the program allow for third-party plug-ins?** - The workflow that is explained through this page relies on the ability for a third-party plug-in to be able to be installed within the application.
* **Is there an API? If, so what are the known limitations?** - Advanced links Typically rely on a third-party API to interact with the software. If an API is not present a text file conversion to IOM will be required, which is likely to be a one-time export.
* **Is element/node selection available through the API?** - Typically the selection of certain parts of the structure is the preferred workflow, but can be worked around.

Other questions should include:
* Does the API use Interfaces or are values extracted as Arrays or Lists?
* Does the program have multiple applications which need to be closed and opened during the different processes?
* How does the application manage the persistence of objects?
* Are results available and how are they provided?
* What is the database Unit's system and can this be changed?


# Opening a Third-Party Application from Checkbot

The addition of external plug-ins to the Checkbot UI (started from the IDEAStatiCa App) is currently limited to IDEA-developed links only, however, we are looking at enabling a public UI interface in the future. Therefore, it is recommended that an App be created in the manner above so that Checkbot is launched and controlled from within the base application UI.

If this type of implementation is in line with a direct import outlined below, a much simpler project framework can be utilized, as syncing is typically neglected for a direct import. The [Ram Link project](https://github.com/idea-statica/ideastatica-public/tree/main/src/bim-links/bentley-ram) follows this methodology.

> Currently this  

# Other ways to link/import a Model into Checkbot
The Checkbot application can be opened from the main IDEAStatiCa App and currently allows the import of a select few model file types including IOM and the SAF (Structural Analysis format) file types. As per above, additional import options cannot be added by third-party developers currently.

