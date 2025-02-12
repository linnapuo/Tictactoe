@description('Location for all resources')
param location string = resourceGroup().location

var appName = 'tic'
var gitRepoUrl = 'https://github.com/linnapuo/Tictactoe'

var uniqueId = uniqueString(resourceGroup().id)
var appServicePlanName = '${appName}-plan-${uniqueId}'
var webAppName = '${appName}-app-${uniqueId}'
var signalrName = '${appName}-signalr-${uniqueId}'
var staticSiteName = '${appName}-swa-${uniqueId}'

resource appServicePlan 'Microsoft.Web/serverfarms@2024-04-01' = {
  name: appServicePlanName
  location: location
  sku: {
    name: 'F1'
  }
}

resource webApp 'Microsoft.Web/sites@2024-04-01' = {
  name: webAppName
  location: location
  properties: {
    serverFarmId: appServicePlan.id
    httpsOnly: true
  }
  identity: {
    type: 'SystemAssigned'
  }
}

resource gitsource 'Microsoft.Web/sites/sourcecontrols@2024-04-01' = {
  parent: webApp
  name: 'web'
  properties: {
    repoUrl: gitRepoUrl
    branch: 'master'
    isManualIntegration: true
  }
}

resource signalr 'Microsoft.SignalRService/signalR@2024-10-01-preview' = {
  name: signalrName
  location: location
  sku: {
    name: 'Free_F1'
  }
  identity: {
    type: 'SystemAssigned'
  }
}

resource staticSite 'Microsoft.Web/staticSites@2024-04-01' = {
  name: staticSiteName
  location: 'westeurope'
  properties: {
    repositoryUrl: gitRepoUrl
    branch: 'master'
    provider: 'GitHub'
  }
  sku: {
    name: 'Free'
  }
}
