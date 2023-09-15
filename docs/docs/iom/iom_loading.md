# Load Cases

Load Cases are the basic entities for which the internal force results are defined. Load Cases can be used directly to generate Load Effects on a Connection, or they can be added together to generate Load Combinations. Apart from the Load Case name, there are several important properties:
* Load Group - please see the next paragraph
* Load Case Type (Permanent, Variable, Accidental, Nonlinear)

# Load Groups

Load Groups group Load Cases together and define their behaviour when assigned under a Load Combination.
Apart from the Load Group name, there are the following properties you need to specify:
* Load Group Type (Permanent, Variable, Accidental, Fatigue, Seismic)
* Relation (Standard, Exclusive, Together) - please see the Load Combinations paragraph to learn about the consequences of this property

# Load Combinations

Load Combinations can be used to combine multiple Load Cases together to act simultaneously and then used to generate Load Effects on a Connection.
You can define the Combination name and Description, which is informative. The important properties are:
* Combination Items (Load Cases with appropriate combination coefficients)
* Combination Type (limit state - ULS or SLS)
* Combination Calculation Type (Evaluation type - Linear or Envelope)

Linear Combination will simply add the specified Load Cases. Envelope Combination will take the Load Groups and their Relation into account, and generate multiple Linear Combinations accordingly in the background.
