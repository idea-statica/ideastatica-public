# Open Model Components

The Open Model Components in Grasshopper allow users to interact flexibly with IDEA Open Model (IOM) in the Grasshopper environment. 

IDEA Open Model is IDEA StatiCa’s own interoperability framework and open file format. It is defined as a modulated .Net (C#) class structure and used to describe FEA or BIM models and designed and developed to interact natively with IDEA StatiCa Apps.

It is good to have a basic understanding on the make-up of IDEA Open Model to understand how the components should work together. For more information on IDEA Open Model please refer to [Open Model Documentation](../../iom/iom_getting_started.md).

## Component Overview

There are two primary types of Open Model components:
* **Object Components** – Object components relate to creation or modification of modulated Open Model objects that can be then assigned to a Model. 
* **Compiling Components** – Compiling components relate to compiled models, results or connection data which can be saved and then imported into IDEA StatiCa Apps. These components have assigned databases of assigned objects. 
