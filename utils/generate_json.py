#!/usr/bin/env python
# coding: utf-8

import json
import argparse


teams = [
    'Alvin Raiders',
    'Angleton Wildcats',
    'Bay Area Buccaneers',
    'Brazosport Longhorns',
    'Dickinson Gators',
    'East End Eagles',
    'Hitchcock Red Raiders',
    'La Porte Texans',
    'League City 49ers',
    'Magnolia Park Sharks',
    'Manvel Texans',
    'Pasadena Panthers',
    'Pearland Patriots',
    'Pearland Texans',
    'Sagemont Cowboys',
    'SE Houston Wildcats',
    'Texas City Stingrays'
]

levels = [
    'Peewee',
    'Freshman',
    'Sophomore',
    'Junior',
    'Senior'
]


def generate_standings_json(output_file):

    standings_dict = {
       "Title": "2024 Standings",
       "Message": "",
       "Standings": []
       }

    for level in levels:
        level_dict = {
            "Level": level,
            "Teams": []
        }

        for team in teams:
            team_dict = {
                "Team": team,
                "Wins": 0,
                "Losses": 0,
                "Ties": 0,
                "Points": 0,
                "Playoff": False,
                "Rank": 0
            }
            level_dict["Teams"].append(team_dict)

        standings_dict["Standings"].append(level_dict)

    with open(output_file, 'w') as f:
        json.dump(standings_dict, f, indent=4)


if __name__ == '__main__':

    # create argument parser to select the generator to run
    parser = argparse.ArgumentParser(description='Generate BAFL JSON file')
    parser.add_argument('generator', type=str, help='Generator to run')
    parser.add_argument('--output', type=str, default='data.json', help='Output file name')
    args = parser.parse_args()

    # if generator is 'standings', generate standings JSON
    if args.generator == 'standings':
        generate_standings_json(args.output)
    else:
        raise ValueError('Invalid generator')
