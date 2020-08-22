CREATE DATABASE EasyBlog
GO

USE EasyBlog
GO

CREATE TABLE UserInformation
(
	id BIGINT PRIMARY KEY IDENTITY,
	name VARCHAR(50) NOT NULL,
	surname VARCHAR(50) NOT NULL,
	email VARCHAR(100) NOT NULL UNIQUE,
	phone VARCHAR(15) NOT NULL,
	createdDate DATE NOT NULL,
	modifiedDate DATE NOT NULL,
	lastLoginDate DATE NOT NULL
)

CREATE TABLE UserLogin
(
	id BIGINT PRIMARY KEY,
	email varchar(100) NOT NULL UNIQUE,
	password VARCHAR(MAX) NOT NULL
	FOREIGN KEY (id) REFERENCES UserInformation(id)
)

CREATE TABLE SocialMedia
(
	id BIGINT PRIMARY KEY IDENTITY,
	name varchar(50) NOT NULL UNIQUE,
	code varchar(50) NOT NULL UNIQUE,
	color varchar(8) NOT NULL
)

CREATE TABLE SocialMediaLink
(
	id BIGINT PRIMARY KEY IDENTITY,
	userID BIGINT FOREIGN KEY REFERENCES UserInformation(id),
	socialMedia BIGINT FOREIGN KEY REFERENCES SocialMedia(id),
	link VARCHAR(200) NOT NULL UNIQUE
)

/*
drop table PortfolioCategoryImageRelationship
drop table PortfolioCategory
drop table Portfolio
drop table ResumeSectionItemExplanations
drop table ResumeSectionItem
drop table ResumeSection
drop table Resume
drop table AboutInformation
drop table About
drop table HomeSubText
drop table Home
drop table NavigationItem
drop table Navigation
drop table Main
drop table Contact
drop table Story
drop table Blog
drop table Template
*/

CREATE TABLE Template
(
	id BIGINT PRIMARY KEY,
	FOREIGN KEY (id) REFERENCES UserInformation(id),
	templateName varchar(20) not null
)

CREATE TABLE Blog
(
	id BIGINT PRIMARY KEY,
	header varchar(50) not null,
	backgroundColor varchar(7) not null,
	FOREIGN KEY (id) REFERENCES Template(id)
)
CREATE TABLE Story
(
	id BIGINT PRIMARY KEY IDENTITY,
	blogID BIGINT FOREIGN KEY REFERENCES Blog(id),
	title varchar(50) not null,
	body varchar(Max) not null,
	image varchar(100) not null,
)

CREATE TABLE Contact
(
	id BIGINT PRIMARY KEY,
	header varchar(50) not null,
	backgroundColor varchar(7) not null,
	address varchar(100),
	city varchar(20),
	state varchar(20),
	country varchar(20),
	phone varchar(15),
	email varchar(100),
	FOREIGN KEY (id) REFERENCES Template(id)
)

CREATE TABLE Main
(
	id BIGINT PRIMARY KEY,
	logo varchar(100) not null,
	title varchar(50) not null,
	titleColor varchar(7) not null,
	textColor varchar(7) not null,
	hoverColor varchar(7) not null,
	FOREIGN KEY (id) REFERENCES Template(id)
)

CREATE TABLE Navigation
(
	id BIGINT PRIMARY KEY,
	logo varchar(100) not null,
	barColor varchar(7) not null,
	FOREIGN KEY (id) REFERENCES Template(id)
)

CREATE TABLE NavigationItem
(
	id BIGINT PRIMARY KEY IDENTITY,
	navID BIGINT FOREIGN KEY REFERENCES Navigation(id),
	content varchar(50) not null,
	sectionName varchar(50) not null,
	priority int not null
)

CREATE TABLE Home
(
	id BIGINT PRIMARY KEY,
	background varchar(MAX) not null,
	textColor varchar(7) not null,
	mainText varchar(100) not null,
	FOREIGN KEY (id) REFERENCES Template(id)
)

CREATE TABLE HomeSubText
(
	id BIGINT PRIMARY KEY IDENTITY,
	homeID BIGINT FOREIGN KEY REFERENCES Home(id),
	subText varchar(100) not null
)

CREATE TABLE About
(
	id BIGINT PRIMARY KEY,
	image varchar(MAX) not null,
	background varchar(7) not null,
	frameColor varchar(7),
	header varchar(50) not null,
	subTitle varchar(50),
	body varchar(Max) not null,
	FOREIGN KEY (id) REFERENCES Template(id)
)

CREATE TABLE AboutInformation
(
	id BIGINT PRIMARY KEY IDENTITY,
	aboutID BIGINT FOREIGN KEY REFERENCES About(id),
	informationTitle varchar(50) not null,
	informationValue varchar(50) not null
)

CREATE TABLE Resume
(
	id BIGINT PRIMARY KEY,
	header varchar(50) not null,
	background varchar(7) not null,
	FOREIGN KEY (id) REFERENCES Template(id)
)

CREATE TABLE ResumeSection
(
	id BIGINT PRIMARY KEY IDENTITY,
	resumeID BIGINT FOREIGN KEY REFERENCES Resume(id),
	header varchar(50) not null,
)

CREATE TABLE ResumeSectionItem
(
	id BIGINT PRIMARY KEY IDENTITY,
	resumeSectionID BIGINT FOREIGN KEY REFERENCES ResumeSection(id),
	header varchar(50) not null,
	date varchar(100),
	location varchar(100),
	explanation varchar(MAX),
)

CREATE TABLE ResumeSectionItemExplanations
(
	id BIGINT PRIMARY KEY IDENTITY,
	resumeSectionItemID BIGINT FOREIGN KEY REFERENCES ResumeSectionItem(id),
	explanation varchar(MAX)
)

CREATE TABLE Portfolio
(
	id BIGINT PRIMARY KEY,
	header varchar(50) not null,
	background varchar(7) not null,
	FOREIGN KEY (id) REFERENCES Template(id)
)

CREATE TABLE PortfolioCategory
(
	id BIGINT PRIMARY KEY IDENTITY,
	portfolioID BIGINT FOREIGN KEY REFERENCES Portfolio(id),
	category varchar(15) not null,
)

CREATE TABLE PortfolioCategoryImageRelationship
(
	id BIGINT PRIMARY KEY IDENTITY,
	portfolioCategoryID BIGINT FOREIGN KEY REFERENCES PortfolioCategory(id),
	image varchar(MAX) not null,
)

