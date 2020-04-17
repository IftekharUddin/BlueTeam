from re import finditer
from itertools import combinations
from random import choice, sample

# these l33t substitutions are taken from the zxcvbn internals, but of course there are many others,
# see: https://simple.wikipedia.org/wiki/Leet
l33t_map = {
    'a': ['4', '@'],
    'A': ['4', '@'],
    'b': ['8'],
    'B': ['8'],
    'c': ['(', '{', '[', '<'],
    'C': ['(', '{', '[', '<'],
    'e': ['3'],
    'E': ['3'],
    'g': ['6', '9'],
    'G': ['6', '9'],
    'i': ['1', '!', '|'],
    'I': ['1', '!', '|'],
    'l': ['1', '|', '7'],
    'L': ['1', '|', '7'],
    'o': ['0'],
    'O': ['0'],
    's': ['$', '5'],
    'S': ['$', '5'],
    't': ['+', '7'],
    'T': ['+', '7'],
    'x': ['%'],
    'X': ['%'],
    'z': ['2'],
    'Z': ['2']
}

l33t_map_count = dict()
for key, val in l33t_map.items():
    l33t_map_count[key] = len(val)


def sub(word, letter, position):
    for l33t_substitution in l33t_map[letter]:
        yield word[:position] + l33t_substitution + word[position+1:]


def sub_all(word, indices):
    res = [word]
    for idx in indices:
        curr_items = []
        for item in res:
            for substitution in sub(item, item[idx], idx):
                curr_items.append(substitution)
        res = curr_items
    return res


def generate_all_l33t(word, mode='random', random_num=1):
    if mode == 'random':
        l33t_indices = [match.start() for letter in l33t_map_count.keys()
                        for match in finditer(letter, word)]

        possibilities = [()] + [item for n in range(1, len(l33t_indices)+1)
                                for item in combinations(l33t_indices, n)]

        random_possibilities = sample(possibilities, random_num)
        for random_poss in random_possibilities:
            res = word
            for idx in random_poss:
                res = res[:idx] + choice(l33t_map[res[idx]]) + res[idx+1:]
            yield res
    elif mode == 'all':
        l33t_indices = [match.start() for letter in l33t_map_count.keys()
                        for match in finditer(letter, word)]

        yield word
        num = 1
        for n in range(1, len(l33t_indices)+1):
            for perm in combinations(l33t_indices, n):
                for item in sub_all(word, perm):
                    num += 1
                    yield item
        # print('{}: {}'.format(word, num))
    else:
        yield word


if __name__ == '__main__':
    for item in generate_all_l33t('apple'):
        print(item)
