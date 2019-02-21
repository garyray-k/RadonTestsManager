# RadonTestManager
<b> An app I'm building for my sister. She wanted a way to better manage an inventory her company has and I thought I could make an app for that. Now I'm too far in to turn back now. </b>

## Summary
I am following along with the book [Hands On Full Stack Development wiht ASP.NET Core](https://www.packtpub.com/web-development/hands-full-stack-web-development-aspnet-core "Packt Wesite") but tailoring it to my needs.
Commit messages may chronicle my musings and lessons learned/objectives achieved. I am using this app as a learning project to test my grit in C# and .NET. I try to work on it at least one hour a day before the family wakes up but generally average 1.5 hours of work/day. I am open to requests/suggestions/comments. 

I am using [Postman](https://www.getpostman.com/) to test my API as I go.

Also am using SQL Server in a [Docker](https://www.docker.com) container on my local machine for testing.

[Swagger](https://swagger.io) is used to document the API.

[GitHubPages](https://garyray-k.github.io)

## Getting Started

I will usually have a `master` and `dev` branch. The `dev` branch will be labeled with the end goal of the branch.

1. Clone down the repo. 
    * switch to desired branch (as of 20190220, the dev branch will auto seed the SQL database)
2. Switch into the directory `cd RadonTestsManager`
3. Docker Compose all the containers (API, SQL) `docker-compose build`
4. Watch your console prompt do a bunch of things (may need to download images before building the app)
5. `docker-compose up` will spin up both the asp.net core app and SQL Server.
6. Keep that terminal window open and find the SQL Server at `127.0.0.1:1433` or browse to the API documentation at `127.0.0.1:5555/swagger`
7. Kill the containers by hitting `Ctrl + C` in the same terminal window you ran it all in. (prompt should say as much)
