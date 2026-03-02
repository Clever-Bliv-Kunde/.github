// This is copied from Clever Platform
// Only edit this if you are sure of what you are doing
// Source: https://github.com/cleveras-platform/.github/tree/main

targetScope = 'subscription'

@allowed([
  'non-production'
  'production'
])
param environment string = 'non-production'

type serviceConnection = {
  name: string
  principalId: string
  existing: bool?
}

param serviceconnections serviceConnection[]

// For each service connection

// Assign within the subscription:
// * Contributor
// * Service Connection
// * Service Connection Deployment Write

var svcsToCreate = filter(serviceconnections, (svc) => svc.?existing == null || svc.?existing != true)

module serviceconnection_roleassignments 'modules/serviceconnection_roleassignments.bicep' = [
  for svcconn in svcsToCreate: {
    name: guid('serviceconnectionRoleAssignments', svcconn.name)
    params: {
      environment: environment
      servicePrincipal: svcconn.principalId
    }
  }
]
