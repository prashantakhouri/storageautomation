BDD.CORE.API
------------

- AzDevOps Sync
  - Use [Spex](https://vamsitp.github.io/spexdocs/)

- Code files
	- `Core\AppTestBase.cs`: Used for "Hooks"

- Configuration
	- `app.config`: Change values under `<spex>` node for Spex (AzDevOps-Sync)
	- Make sure the values of DefaultAssignedTo (in .config) / @owner tag (in .feature) are valid.
		- e.g.: The alias vamsitp(@microsoft.com) is different than vamsi.tp(@microsoft.com) - though both are valid aliases. AzDevOps only honors that one that was added to the account

- Main classes to use
    - `ApiStepDefinitionBase`: To add additional functionality, inherit this class and add/override methods
    - `ApiExecutor`: To add additional functionality, inherit this class and add/override methods

- Scenario specific Packages
    - `Bdd.Core.Web`: For Web Tests
    - `Bdd.Core.Api`: For Api Tests