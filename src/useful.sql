/****** Script for SelectTopNRows command from SSMS  ******/
select count(*) from PgnGames
select count(*) from Games
select count(*) from Players
select count(*) from Sites
select count(*) from Events

select *, len(lastname) from players 
where lastname = 'Fors'



/*
delete from pgngames
delete from pgnimports


  delete from EventLookup
  delete from SiteLookup
  delete from PlayerLookup
  delete from games
  delete from players
  delete from events
  delete from sites
  delete from ImportedPgnGameIds
*/
