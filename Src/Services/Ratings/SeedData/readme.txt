CREATE CONSTRAINT FOR (p:Person) REQUIRE p.name IS UNIQUE;

CREATE (:Person {name : 'Michael Caine', born:1964});
CREATE (:Movie {title: 'Batman Begins'});
CREATE (:Movie {title: 'The Man Who Would Be King'});

MATCH (a:Person), (m:Movie)
WHERE a.name = 'Michael Caine' AND m.title = 'Batman Begins'
CREATE (a)-[rel:ACTED_IN {roles: ['Bruce Wayne', 'Batman']}]->(m)
RETURN a, m;

MATCH (a:Person), (m:Movie)
WHERE a.name = 'Michael Caine' AND m.title = 'The Man Who Would Be King'
CREATE (a)-[rel:ACTED_IN {roles: ['Peachy Carnehan']}]->(m)
RETURN a, m;


MATCH (a:Person)
WHERE a.name = 'Michael Caine'
SET a.dateCreated = date("2019-09-30");

MATCH (a:Person)
WHERE a.dateCreated = date("2019-09-30")
RETURN a;

MATCH (n)
RETURN count(n) as count;


MATCH (a:Person)-[r:ACTED_IN]->(k)
WHERE a.name = 'Michael Caine'
RETURN r,k;

//Delete all
MATCH (n)
DETACH DELETE n;


