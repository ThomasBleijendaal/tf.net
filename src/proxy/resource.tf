# resource "sampleprovider_file" "test" {
#      id = "fdsa"
# }

data "sampleprovider_folder" "folder" {
    path = "./"
}

output x {
    value = data.sampleprovider_folder.folder.files
}