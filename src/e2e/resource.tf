resource "sampleprovider_file" "demo_file" {
  path = "./file.txt"
  content = "fdsafdsa"
}

resource "sampleprovider_file" "demo_file_2" {
  path = "./file2.txt"
  content = "fdsa"
}

data "sampleprovider_folder" "folder" {
  path = "./"
}

output test {
  value = data.sampleprovider_folder.folder.files
}

# output concat {
#   value = provider::sampleprovider::sampleprovider_concat("f", "a")
# }
