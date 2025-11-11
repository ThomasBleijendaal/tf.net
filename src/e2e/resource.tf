resource "sampleprovider_file" "demo_file" {
  path = "./file.txt"
  content = "fdsafdsa"
}

resource "sampleprovider_file" "demo_file_2" {
  path = "./file2.txt"
  content = "fdsa"
}
