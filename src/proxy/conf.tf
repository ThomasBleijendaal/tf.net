# provider "sampleprovider" {
# }

terraform {
  # required_providers {
  #   sampleprovider = {
  #     source = "example.com/example/sampleprovider"
  #     version = "1.0.0"
  #   }
  # }
}

terraform {
  backend "http" {
    address = "http://localhost:5346/.tf-storage/sample"
    lock_address = "http://localhost:5346/.tf-storage/sample"
    unlock_address = "http://localhost:5346/.tf-storage/sample"
    username = "-"
    password = "fdsa"
  }
}

