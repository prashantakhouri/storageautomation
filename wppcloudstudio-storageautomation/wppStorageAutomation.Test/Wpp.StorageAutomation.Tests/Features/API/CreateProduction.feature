@api @owner=saloni.shrivastava @testplan=280 @testsuite=285 @parallel=false
Feature: CreateProduction
A Production will be created in the Azure File Share

@testcase=436 @createproduction @bvt @DeleteProduction @priority=1 @version=6
Scenario: Verify a user can create a production
When  I send the "Create Production" API with below data for "ProductionStore1" Productionstore
| Key                       | Value             |
| tokens[0].productionToken | Test-Production_1 |
Then I receive valid HTTP response code "200"
And Verify Production "Test-Production_1" is created in "ProductionStore1" production store in "5" seconds
And I receive sync response for 'Create Production' API
And Verify "success" sync response body for "Create Production" API
| Key  | Value             |
| name | Test-Production_1 |
| uri  | Test-Production_1 |

@testcase=437 @createproduction @priority=3 @version=3
Scenario Outline: Verify production is not created if the name contains invalid characters
When I send the "Create Production" API with "tokens[0].productionToken" with value "<productiontoken>" for "ProductionStore1" Productionstore
Then I receive valid HTTP response code "422"

Examples:
| productiontoken |
| ProdWith \      |
| ProdWith *      |
| ProdWith /      |
| ProdWith \|     |
| ProdWith :      |
| ProdWith <      |
| ProdWith >      |
| ProdWith ?      |

@testcase=2490 @createproduction @priority=2 @DeleteProduction @version=6 @testplan=280 @testsuite=2413
Scenario: Verify duplicate production creation is not allowed
Given I send the "Create Production" API with below data for "ProductionStore1" Productionstore
| Key                       | Value                   |
| tokens[0].productionToken | MicrosoftAdCampaignAuto |
Then Verify Production "MicrosoftAdCampaignAuto" is created in "ProductionStore1" production store in "5" seconds
When I send the "Create Production" API with "tokens[0].productionToken" with value "MicrosoftAdCampaignAuto" for "ProductionStore2" Productionstore
Then Verify "failed" sync response with "422-UnprocessableEntity" for "statusCode"

@testcase=439 @createproduction @priority=2 @version=5
Scenario: Verify subfolders in same level with same name are not allowed
When  I send the "Create Production" API with below data for "ProductionStore1" Productionstore
| Key                                           | Value                  |
| tokens[0].productionToken                     | MicrosoftAdCampaignSub |
| directoryTree[0].subitems[0].subitems[0].path | images                 |
| directoryTree[0].subitems[0].subitems[1].path | images                 |
Then Verify Production "MicrosoftAdCampaignSub" is not created in "ProductionStore1" production store
Then Verify "failed" sync response with "422-UnprocessableEntity" for "statusCode"
When  I send the "Create Production" API with below data for "ProductionStore1" Productionstore
| Key                                           | Value                    |
| tokens[0].productionToken                     | ProductionForSameSubName |
| directoryTree[0].subitems[0].subitems[0].path | images                   |
| directoryTree[0].subitems[0].subitems[1].path | IMAGES                   |
Then Verify Production "ProductionForSameSubName" is not created in "ProductionStore1" production store
Then Verify "failed" sync response with "422-UnprocessableEntity" for "statusCode"

@testcase=440 @createproduction @priority=3 @version=4
Scenario: Verify production name length should not be more than 255 characters
When  I send the "Create Production" API with below data for "ProductionStore1" Productionstore
| Key                       | Value                                                                                                                                                                                                                                                               |
| tokens[0].productionToken | QVHphwBH8sS5tM6ZhPb5WhD9Q10hS8xiC1856hnPedXWRP22iTYKErTM7MxFyJn7C3RSPmQbAsxtxyjLes3pAtaXfbosTZiDKDDw6fHIIoPzz3WDCuGhGBfbDD3altjW1OtCSCLs7KfAbO9ZJgpdN76lIQVVO610uC4y3wVxOJgTYQXQfuy7lqgXw9rLMNIM0CHb3n5eHmdCNyerugFE7DmV5pnML3E61gusfsGxRKSC6bneBY6iQtVvThBJlR0mUs4 |
Then I receive valid HTTP response code "422"

@testcase=911 @createproduction @priority=3
Scenario: Verify production name length should not be less than 8 characters
When  I send the "Create Production" API with below data for "ProductionStore1" Productionstore
| Key                       | Value |
| tokens[0].productionToken | Test8 |
Then I receive valid HTTP response code "422"
#Given HTTP client is not authenticated
#When  I send the "Create Production" API with below data
#| Key  | Value  |
#| productiontoken | MicrosoftAdCampaign |
#Then Then I receive valid HTPP response code 401
#And Response body "POST" is non-empty
#Given HTTP client is not authorised
#When  I send the "Create Production" API with below data
#| Key  | Value  |
#| productiontoken | MicrosoftAdCampaign |
#Then Then I receive valid HTPP response code 403
#And Response body "POST" is non-empty
