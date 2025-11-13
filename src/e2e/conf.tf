provider "sampleprovider" {
  file_header = "abc"
}

terraform {
  required_providers {
    sampleprovider = {
      source = "example.com/example/sampleprovider"
      version = "1.0.0"
    }
  }
}
