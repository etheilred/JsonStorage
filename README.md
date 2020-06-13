# JsonStorage
Tiny library for storing data described by models

## Repos structure

* JsonStorage - library itself
* Tests - unit tests project
* Tester - console app demo

## Usage

```csharp
using (var provider = new StorageProvider("path_to_storageFolder")) {
 // work with storage inside using block
}
```
Consider the following class:
```csharp
class User {
  string Name {get;set;}
  string Surname {get;set;}
  int Id {get;set;}
}
```

* `provider.Write(new User() {Name = "name", Surname = "surname"})` will add new user to table of Users (you can provide specific name as a second parameter). If table does not exist, a new table will be created.
If type has property `public int Id {get;set;}` it will be automatically set to unique id

* `provider.Write<User>(new [] {new User(), new User()})` - the same, but with range of objects

* `provider.Get<User>(x => x.Name == "name", "Usernames")` - looks for a User in table of User (table called "Usernames") and returns it

* `provider.GetTable<User>("Usernames")` - returns table as IEnumerable<User>

* `provider.Delete<User>(x => x.Id == 1)` - deletes first occurence of such element

* `provider.DeleteAll<User>()` - clears tables (removes all elements, but leaves the table itself)
