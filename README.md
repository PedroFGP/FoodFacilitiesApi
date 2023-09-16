
# Food Facilities API Challenge

## Introduction

This project was a coding challenge with the objective of building an API (Application Programming Interface) to serve a database with the possibility of different filters to the web.

[Database Link](https://data.sfgov.org/Economy-and-Community/Mobile-Food-Facility-Permit/rqzj-sfat/data)

This project accomplishes this by implementing an ASP.NET Web Api using the hexagonal architecture and clear definition and separation between projects in order to make this project scalable, concise, easy to maintain and expand, following more modern approaches to building software.

## Technical Aspects

The chosen architecture was the hexagonal approach, since it offers the following benefits:

- Decoupled by nature segregating the business logic, the domain and the adapters (driving and driven)
- Having a decoupled nature makes it easy to change services providers on the fly (Cloud Providers, Third party API's, Vendors, etc...)
- Easy to scale (since the adapters would be separated in different and projects and can be scaled according to demand)
- "Modern" architecture (although envisioned in 2005), it's been trending lately

***Observation**: for this project, this kind of architecture is a bit overkill (and really verbose) but keeping the future in mind it does make sense. If the project had no intentions of growing and would remain simple, there are other architectures to be considered, such as **Vertical Slice** coupled with **Minimal API**.*

## Improvements

There are few areas where there is room for improvement:

- Add a cache system (redis, etc...) for queries based on parameters in order to make the response time faster (currently it's not slow, but the database is miniscule, and the queries are simple)
- A better definition and slim down version of the DTO returned by the API
- Improve the swagger documentation by adding the schemas definitions, example values for parameters and more detailed responses
- Use a proper database engine (import the csv file to a SQL/PostgreSql database and use EF as the ORM)
- Add more scenarios to unit testing
- Add githooks to execute the tests before committing

### Trade-offs

As been discussed before in [Technical Aspects](#technical-aspects), this architecture is a bit overkill for the task as it is, so probably should have taken a simpler route.

### Scaling & Future

As it stands, this project should be able to scale as the number of users rise. The only weak point being the database (csv file).

## Building

To use this project just clone the repository, restore nuget packages and build the solution.

## Testing

There are several ways that the tests can be executed, as it stands this project supports the following:

- Using the "Test" section in Visual Studio to run the tests (which offers debug, run single tests, etc...)
- Running in a terminal: 

```bash
dotnet test "./FoodFacilities.Test/FoodFacilities.Test.csproj"
```

***Observation**: As discussed in [Improvements](#improvements) this can be improved by using githooks to run the tests before committing, etc...*

## Time Spent

The total hours spent on this project (coding, research, documentation, testing) is 18 hours.

This can be roughly divided in the following areas:

- Research/Coding: 10 hours
- Documentation: 2 hours
- Testing (developing tests): 6 hours
