## Copyright (c) Microsoft Corporation. All rights reserved.
## Customer - WPP Cloud Studio(WPP)

## Introduction 
This repository consists of all the terraform modules, scripts and yaml files to create the infrastructure for the desired environment. 

## Modules Structure
Each module consists of the following : 
1.	main.tf
2.	output.tf
3.	providers.tf
4.	readme.md(module specific definition)
5.  variables.tf
6.  authentication.tf(not mandatory for every module)

## Calling the generalized modules
The module_master.tf file in wpp-core-infra folder calls all the modules from the modules folder as per the requirement to develop the infrastructure. The variables.tf file in the same folder consists of all the defined variables(uninitialized) that will be provided to the called modules.
