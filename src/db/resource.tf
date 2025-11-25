resource "dbprovider_user" "admin" {
  name = "admin"
  email = "a@b.c"
  roles =  [] #[dbprovider_role.reader.id, dbprovider_role.writer.id, dbprovider_role.owner.id]
}

# resource "dbprovider_role" "reader" {
#   name = "reader"
# }

# resource "dbprovider_role" "writer" {
#   name = "writer"
# }

# resource "dbprovider_role" "owner" {
#   name = "owner"
# }
