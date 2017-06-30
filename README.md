# C# Client for Yousign API

## Description

This client allows to use the Yousign SOAP API through .NET

## Getting started

 var cosignClient = new CosignClient(yousignKey, yousignUsername, yousignPassword, isProdVersion);
 var result = cosignClient.Execute(client => YourMethod(client, param1, param2)));
