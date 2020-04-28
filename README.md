# Trojan Blitz
A highly versatile, fast-paced, deck-building card game for the mobile platform. Intuitive play with deck building strategy to compete in battles on the clock. Players choose their character at the start of a match from a randomly generated selection, and then play using the deck they have customized with their own cards.

## Gameplay
[Video](https://streamable.com/8qizpb)

## Multiplayer Game - System Design
Trojan Blitz, created with Unity, written in C#, is a multiplayer game application, that is designed to run in mobile devices. Using remote procedure calls (RPCs), it propagates a state of the application to player nodes in the network. It might as well have been architected in such a way that it only broadcasts "action" to the participating nodes. Other than multi-agent element, Trojan Blitz has an architecture typically expected of game applications. GameManager, Character, Card, Player are fundamental classes that make up this game.

## CSCI599
This project is initiated as the course assignment of CSCI599 - Special Topics (Social Mobile Games), at the University of Southern California in Spring 2020.
