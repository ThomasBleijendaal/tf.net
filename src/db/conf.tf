provider "dbprovider" {
  
}

terraform {
  required_providers {
    dbprovider = {
      source = "example.com/example/dbprovider"
      version = "1.0.0"
    }
  }
}
