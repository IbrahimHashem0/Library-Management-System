use [Library Management System]

--Create the table
CREATE TABLE Quotes (
    Id INT PRIMARY KEY IDENTITY(1,1),
    QuoteText NVARCHAR(MAX) NOT NULL,
    Author NVARCHAR(255) NOT NULL
);

--Insert the library quotes
INSERT INTO Quotes (QuoteText, Author) VALUES 
('I have always imagined that Paradise will be a kind of library.', 'Jorge Luis Borges'),
('The only thing that you absolutely have to know, is the location of the library.', 'Albert Einstein'),
('When in doubt, go to the library.', 'J.K. Rowling'),
('A library is not a luxury but one of the necessities of life.', 'Henry Ward Beecher'),
('Google can bring you back 100,000 answers. A librarian can bring you back the right one.', 'Neil Gaiman'),
('To build up a library is to create a life. It’s never just a random collection of books.', 'Carlos María Domínguez'),
('A library is a place where you can lose your innocence without losing your virginity.', 'Germaine Greer'),
('If you have a garden and a library, you have everything you need.', 'Marcus Tullius Cicero'),
('Without libraries what have we? We have no past and no future.', 'Ray Bradbury'),
('Whatever the cost of our libraries, the price is cheap compared to that of an ignorant nation.', 'Walter Cronkite'),
('I received the fundamentals of my education in school, but that was not enough. My real education, the superstructure, the details, the true architecture, I got out of the public library.', 'Isaac Asimov'),
('The library is the temple of learning, and learning has liberated more people than all the wars in history.', 'Carl T. Rowan'),
('A library implies an act of faith.', 'Victor Hugo'),
('Libraries will get you through times of no money better than money will get you through times of no libraries.', 'Anne Herbert'),
('A library is a good place to go when you feel unhappy, for there, in a book, you may find encouragement and comfort.', 'E.B. White'),
('The very existence of libraries affords the best evidence that we may yet have hope for the future of man.', 'T.S. Eliot'),
('A great library contains the diary of the human race.', 'George Dawson'),
('Libraries are the memory of humankind.', 'Digital Library Federation'),
('In the nonstop tsunami of global information, librarians provide us with floaties and teach us to swim.', 'Linton Weeks'),
('When I got my library card, that’s when my life began.', 'Rita Mae Brown'),
('Bad libraries build collections, good libraries build services, great libraries build communities.', 'R. David Lankes'),
('The library is the worst group of people ever assembled with the best intentions.', 'Lemony Snicket'),
('Libraries are not made; they grow.', 'Augustine Birrell'),
('Google can bring you back 100,000 answers. A librarian can bring you back the right one.', 'Neil Gaiman'),
('Perhaps no place in any community is so totally democratic as the town library. The only entrance requirement is interest.', 'Lady Bird Johnson'),
('Cutting libraries during a recession is like cutting hospitals during a plague.', 'Eleanor Crumblehulme'),
('The only thing that you absolutely have to know, is the location of the library.', 'Albert Einstein'),
('A library is an arsenal of liberty.', 'Anonymous'),
('Nothing is pleasanter than exploring a library.', 'Walter Savage Landor');