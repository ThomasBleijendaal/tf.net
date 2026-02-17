resource "sampleprovider_file" "demo_file" {
  path = "./file.txt"
  content = "fdsafdsa"

  property {
    key = "Key1"
    value = "Value1"
  }
  property {
    key = "Key2"
    value = "Value2"
  }
}

# resource "sampleprovider_file" "demo_file_2" {
#   path = "./file2.txt"
#   content = "fdsa"

#   property {
#     key = "Key1"
#     value = "Value1"
#   }
#   property {
#     key = "Key2"
#     value = "Value2"
#   }
# }

# data "sampleprovider_folder" "folder" {
#   path = "./"
# }

# output test {
#   value = data.sampleprovider_folder.folder.files
# }

# output test2 {
#   value = provider::sampleprovider::sampleprovider_policy1("test", 1)
# }

# output concat {
#   value = provider::sampleprovider::sampleprovider_concat("f", "a")
# }
