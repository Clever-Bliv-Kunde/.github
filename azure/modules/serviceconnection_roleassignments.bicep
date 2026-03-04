// This is copied from Clever Platform
// Only edit this if you are sure of what you are doing
// Source: https://github.com/cleveras-platform/.github/tree/main

targetScope = 'subscription'

extension 'br:mcr.microsoft.com/bicep/extensions/microsoftgraph/v1.0:1.0.0'

param environment string
param servicePrincipal string

var ServiceConnectionRoleDefinitionIDs = {
  production: {
    contributor: '/subscriptions/30d581b0-94bf-4981-8a7c-27fa3f4a7e6c/providers/Microsoft.Authorization/roleDefinitions/aad69d57-27a9-5d65-bfd5-91cbeea59545'
    service_connection: '/subscriptions/30d581b0-94bf-4981-8a7c-27fa3f4a7e6c/providers/Microsoft.Authorization/roleDefinitions/bd03f7ee-20c5-5c8a-96fa-8fd08a2614bd'
    svcconn_deployment_writer: '/subscriptions/30d581b0-94bf-4981-8a7c-27fa3f4a7e6c/providers/Microsoft.Authorization/roleDefinitions/cdfed6ef-5a8b-5dcc-9a7e-4a9ed20e3530'
  }
  'non-production': {
    contributor: '/subscriptions/4e6180e0-a0e9-46d7-aae3-890fb2e46404/providers/Microsoft.Authorization/roleDefinitions/78057db6-27a2-5197-87e1-5bbbdbc94333'
    service_connection: '/subscriptions/4e6180e0-a0e9-46d7-aae3-890fb2e46404/providers/Microsoft.Authorization/roleDefinitions/cdd277bf-bc72-56d6-b579-cef6f93c4eca'
    svcconn_deployment_writer: '/subscriptions/4e6180e0-a0e9-46d7-aae3-890fb2e46404/providers/Microsoft.Authorization/roleDefinitions/e75b5651-1027-5b6a-81d4-15afefe30157'
  }
}

var groupCloudApplicationAdministrator = 'sec-accessrole-serviceconnections-cloud_application_administrator_role'

var roledefinitions = ServiceConnectionRoleDefinitionIDs[environment]

resource contributorRA 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(servicePrincipal, roledefinitions.contributor)
  properties: {
    roleDefinitionId: roledefinitions.contributor
    principalId: servicePrincipal
    principalType: 'ServicePrincipal'
  }
}

resource serviceConnectionRA 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(servicePrincipal, roledefinitions.service_connection)
  properties: {
    roleDefinitionId: roledefinitions.service_connection
    principalId: servicePrincipal
    principalType: 'ServicePrincipal'
  }
}

resource svcconnDeploymentWriterRA 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(servicePrincipal, roledefinitions.svcconn_deployment_writer)
  properties: {
    roleDefinitionId: roledefinitions.svcconn_deployment_writer
    principalId: servicePrincipal
    principalType: 'ServicePrincipal'
  }
}

resource cloudAppAdminGroup 'Microsoft.Graph/groups@v1.0' = {
  uniqueName: groupCloudApplicationAdministrator
  displayName: 'Sec - AccessRole - ServiceConnections - Cloud Application Administrator Role'
  mailEnabled: false
  mailNickname: 'sec-accessrole-svcconn-cloudappadmin'
  securityEnabled: true
  members: {
    relationshipSemantics: 'append'
    relationships: [
      servicePrincipal
    ]
  }
}
