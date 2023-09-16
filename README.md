# Food Facilities API Challenge

## Introduction

There is a database of food facilities in San Francisco that should be made available to consult through an API (Application Programming Interface)

[Database Link](https://data.sfgov.org/Economy-and-Community/Mobile-Food-Facility-Permit/rqzj-sfat/data)

This project accomplishes this objetive by implementing an ASP.NET Web Api using the hexagonal architecture and clear definition and separation between projects in order to make this project scalable, concise, easy to maintain and expand and following more moddern aproaches to building software.

## Technical Aspects

The choosen architecture was the hexagonal approach since it offers the following benefits:

- Decoupled by nature segregating the business logic, the domain and the adapters (driving and driven)
- Having a decoupled nature makes it easy to change services providers on the fly (Cloud, Third party API's, vendors, etc...)
- Easy to scale (since the adapters would be separated in different and projects and can be scalled according to demand)
- "Modern" architecture (although envisioned in 2005), it's been trending lately

***Observation**: for this project, this kind of architecture is a bit overkill (and really verbose) but keeping the future in mind it does make sence. If the project had no intentions to grow and would remain simple there are other architectures to be considered such as **Vertical Slice** coupled with **Minimal API**.*

## Improvements

If there was more time available there are a few areas where there is room for improvement:

- Add a cache system (redis, etc...) for queries based on parameters in order to make the response faster (currently is not slow but the database is miniscule and the queries are simple).
- A better definiton and slim down of the DTO returned in the API.
- Improve the swagger documentation by adding the schemas definition, example values for parameters and more detailed responses.
- Use a proper database engine for the job (import the csv file to a SQL/PostgreSql database and use EF as the orm for the queries).
- Add more scenarios to unit testing
- Add githooks to execute the tests before commiting

### Trade-offs

As been discussed before, this architecture is a bit overkill for the task as it is so probably should have taken a simpler route.

### Scaling & Future

As it stands this project should be able to scale as the number of users rise. The only weak point being the database (csv file).

## Building

To use this project just clone the repository, restore nuget packages and build the solution.

## Testing

There are several ways that the tests can be executed, as it stands this project supports the following:

- Using the "Test" section in visual studio to run the tests (which offers debug, run single tests, etc...)
- Running in a terminal: 

```bash
exec dotnet test "/FoodFacilitiesApi/FoodFacilities.Test/FoodFacilities.Test.csproj"
```

***Observation**: As discussed in the [Improvements](#improvements) secion this can be improved by using githooks do run the tests before commiting, etc...*

## Time Spent

The total hours spent on this project (coding, research, documentation, testing) is 18 hours.

This can be roughly divided in the following areas:

- Research/Coding: 10 hours
- Documentation: 2 hours
- Testing (developping tests): 6 hours