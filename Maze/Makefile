CC = dotnet

SOLUTION = Maze.sln
APP_PROJECT = Maze.App/Maze.App.csproj

NAME = Maze
CONFIG = Release

PUBLISH_DIR = ./publish
INSTALL_DIR ?= ./install

all:
	$(CC) build $(SOLUTION) -c $(CONFIG)

clean:
	$(CC) clean $(SOLUTION)
	rm -rf $(PUBLISH_DIR)
	rm -rf $(INSTALL_DIR)
	find . -type d \( -name "bin" -o -name "obj" \) | xargs rm -rf

install: all
	$(CC) publish $(APP_PROJECT) -c $(CONFIG) -o $(PUBLISH_DIR)
	mkdir -p $(INSTALL_DIR)
	cp -r $(PUBLISH_DIR)/* $(INSTALL_DIR)/

uninstall:
	rm -rf $(INSTALL_DIR)

tests:
	$(CC) test $(SOLUTION) -c $(CONFIG)

dvi:
	doxygen Doxyfile

dist: clean
	mkdir -p dist
	cp -r Maze.App Maze.Core Maze.Infrastructure Maze.Tests Maze.sln Makefile dist/
	tar -czf $(NAME).tar.gz dist
	rm -rf dist

.PHONY: all clean install uninstall tests dvi dist