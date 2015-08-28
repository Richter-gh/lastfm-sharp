# Conventions #

In writing lastfm-sharp I tried to follow the conventions for the difference between properties and methods [(link)](http://msdn.microsoft.com/en-us/library/bzwdh01d(VS.71).aspx#cpconpropertyusageguidelinesanchor1) as much as possible:
  * Use a property when the member is a logical data member.
  * Use a method when:
    * The operation is a conversion, such as Object.ToString.
    * The operation is expensive enough that you want to communicate to the user that they should consider caching the result.
    * Obtaining a property value using the get accessor would have an observable side effect.
    * Calling the member twice in succession produces different results.
    * The order of execution is important. Note that a type's properties should be able to be set and retrieved in any order.
    * The member is static but returns a value that can be changed.
    * The member returns an array. Properties that return arrays can be very misleading. Usually it is necessary to return a copy of the internal array so that the user cannot change internal state. This, coupled with the fact that a user can easily assume it is an indexed property, leads to inefficient code. In the following code example, each call to the Methods property creates a copy of the array. As a result, 2n+1 copies of the array will be created in the following loop.

All the opertaions that require interaction with the Last.fm servers (therefore it requires a little extra time for making contact) have been utilized in methods, while all the operations that return static data are utilized as properties.
If you find that something has slipped by me and does not follow this convention, please let me know by [filing an issue](http://code.google.com/p/lastfm-sharp/issues/entry) at the issue tracker.