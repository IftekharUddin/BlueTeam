# a file which produces easy, medium, and hard bad passwords as well as a list of good passwords

from l33t import generate_all_l33t
from zxcvbn import zxcvbn
from itertools import permutations, chain
from random import choice, sample, shuffle, randint, seed

seed()

# define some constants
BAD_PASSWORD_SCORE = 2  # password has to be <= this value to be considered "bad"
MAX_PASSWORD_LENGTH = 24  # password cannot be longer than this value
GOOD_PASSWORD_SCORE = 4  # password has to be >= this value to be considered "good"
# punctuation which can be added to the end of a password
PUNCTUATION = list("!\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~")
# numbers which can be added to the end of a password
NUMBERS = list('0123456789')


def get_password_file(file_name, prefix='../../PasswordDictionaries/'):
    '''
    Gets a list of words from a text file with name file_name and located relative to this file according to the prefix
    '''
    with open(f'{prefix}/{file_name}', 'rt') as fp:
        text = fp.read()
    return text.split('\n')


def get_password_score(password):
    '''
    Returns the zxcvbn password checker's scoring (a number between 0 - terrible - and 4 - awesome)
    See: https://github.com/dropbox/zxcvbn
    '''
    return zxcvbn(password)['score']


def is_bad_password(password):
    '''
    Tells whether a password is "bad" according to our estimation: 
    - it must be shorter than a certain length (so as not to run too long on the platforms)
    - it must be below a certain score
    '''
    if len(password) == 0:
        return False
    return len(password) <= MAX_PASSWORD_LENGTH and get_password_score(password) <= BAD_PASSWORD_SCORE


def is_good_password(password):
    '''
    Tells whether a password is "good" according to our estimation:
    - it must be shorter than a certain length (so as to fit on the platorms)
    - it must be above a certain score
    '''
    if len(password) == 0:
        return False
    return len(password) <= MAX_PASSWORD_LENGTH and get_password_score(password) >= GOOD_PASSWORD_SCORE


def get_bad_passwords_from_iterable(iterable, mode='all', limit=int(1e3)):
    '''
    Gets all the bad passwords out of an iterable
    Parameter mode governs whether we should get all of them or should get a random number of them
    '''
    if mode == 'all':
        # this is the structure of a generator expression (sort of like a list comprehension)
        # see: https://www.python.org/dev/peps/pep-0289/
        return (word for word in iterable if is_bad_password(word))
    elif mode == 'rand':
        rand_idx = [randint(0, len(iterable) - 1) for _ in range(limit)]
        return (iterable[idx] for idx in rand_idx if is_bad_password(iterable[idx]))
    else:
        return ()


def get_all_good_passwords(iterable):
    ''' 
    Gets all good passwords out of an iterable 
    '''
    return (word for word in iterable if is_good_password(word))

# QUESTION/TODO: should we include capitalizations? (passwords often require them)
# def captialize_list(lst):
#     return [word.capitalize() for word in lst]


def generate_permutations(iterable, number, mode='limit', amount=1e4):
    '''
    Generates r-permutations (where the number parameter governs the r); that is, this function returns 
    len(iterable)Pr (or, at the least, some number of them)
    '''
    if mode == 'limit':
        # get a certain number (governed by amount) of permutations
        len_iter = len(iterable) - 1
        for _ in range(int(amount)):
            random_indices = [randint(0, len_iter) for _ in range(number)]

            shuffle(random_indices)

            random_values = [iterable[idx] for idx in random_indices]
            yield ''.join(random_values)
    elif mode == 'all':
        # get all permutations (from Python's itertools: https://docs.python.org/2/library/itertools.html)
        perms = permutations(iterable, number)
        return (''.join(perm) for perm in perms)
    else:
        return ()


def combine_two_lists_into_bad_passwords(iterable_one, iterable_two, mode='limit', limit=int(1e3)):
    '''
    Gets the combination of two lists (specifically, used in getting bad passwords)
    '''
    if mode == 'all':
        # return all possible combinations
        for item_one in iterable_one:
            for item_two in iterable_two:
                possibilities = [item_one + item_two, item_two + item_one,
                                 item_one.capitalize() + item_two.capitalize(),
                                 item_two.capitalize() + item_two.capitalize()]
                for poss in possibilities:
                    if is_bad_password(poss):
                        yield poss
    elif mode == 'limit':
        # return a limit of combinations in
        for _ in range(limit):
            item_one = choice(iterable_one)
            item_two = choice(iterable_two)
            possibilities = [item_one + item_two, item_two + item_one,
                             item_one.capitalize() + item_two.capitalize(),
                             item_two.capitalize() + item_one.capitalize()]
            for poss in possibilities:
                if is_bad_password(poss):
                    yield poss
    else:
        return ()


def all_bad_l33ts(iterable):
    '''
    Get l33tings from an iterable which are bad passwords 
    Note that here this delegates to the generate_all_l33t function which l33ts are gotten (look at l33t.py for more)
    '''
    return (l33t_item for item in iterable for l33t_item in generate_all_l33t(item) if is_bad_password(l33t_item))


def all_good_l33ts(iterable):
    '''
    Get l33tings from an iterable which are good passwords 
    Note that here this delegates to the generate_all_l33t function which l33ts are gotten (look at l33t.py for more)
    '''
    return (l33t_item for item in iterable for l33t_item in generate_all_l33t(item) if is_good_password(l33t_item))


def reverse_string(string):
    '''
    Python idiom for reversing a string
    '''
    return string[::-1]


def get_bad_reversed(iterable):
    '''
    Get reversed items which are badd passwords from an iterable
    '''
    return (reverse_string(item) for item in iterable if is_bad_password(item))


def append_punctuation(iterable, number=1, func=is_bad_password):
    '''
    Appends a certain number of different punctuation to each password in an iterable
    '''
    random_punct = sample(PUNCTUATION, number)
    return (item + punct for item in iterable for punct in random_punct if func(item+punct))


def append_numbers(iterable, func=is_bad_password):
    '''
    Append anywhere from 1 to 5 random numbers to the end of a password
    '''
    for item in iterable:
        rand_num = choice(NUMBERS)
        if func(item + rand_num):
            yield item + rand_num
        for i in range(2, 6):
            rand_numbers = [choice(NUMBERS) for _ in range(i)]
            shuffle(rand_numbers)
            curr_numbers = ''.join(rand_numbers)
            if func(item + curr_numbers):
                yield item + curr_numbers


english_words = get_password_file('english.lst')
female_names = get_password_file('female_names.lst')
male_names = get_password_file('male_names.lst')
zxcvbn_passwords = get_password_file('passwords.lst')
surnames = get_password_file('surnames.lst')
# us_tv_film = get_password_file('us_tv_and_film.lst')

print('Number of english words: {}'.format(len(english_words)))
print('Number of female names: {}'.format(len(female_names)))
print('Number of male names: {}'.format(len(male_names)))
print('Number of surnames: {}'.format(len(surnames)))
print('Number of zxcvbn passwords: {}'.format(len(zxcvbn_passwords)))
print('')

#################################
## GOOD PASSWORDS              ##
#################################


def get_good_passwords_base():
    '''
    Good passwords are 3 or 4 random english words, l33ted.
    This is separated into this function b/c originally thought that words would be potentially l33ted, but found
    that passwords don't pass muster if they are not scrambled a bit
    '''
    three_words = get_all_good_passwords(
        generate_permutations(english_words, 3, amount=1e5))
    four_words = get_all_good_passwords(
        generate_permutations(english_words, 4, amount=1e5))

    return chain(three_words, four_words)


def get_good_passwords():
    return all_good_l33ts(get_good_passwords_base())


print('Writing good passwords: ', end='', flush=True)
with open('GOOD_PASSWORDS.txt', 'wt') as fp:
    num = 0
    for pw in get_good_passwords():
        # print(pw)
        if num % 1000 == 0:
            print('.', end='', flush=True)
        fp.write(pw + '\n')
        num += 1

print('')
print('Number of good passwords: {}'.format(num))

#################################
## EASY (BAD) PASSWORDS        ##
#################################


def get_easy_bad_passwords(mode='all', limit=int(1e3)):
    '''
    An easy bad password can fulfill the following criteria:
    '''
    # 1. a zxcvbn built-in password
    bad_zxcvbn_passwords = get_bad_passwords_from_iterable(
        zxcvbn_passwords, mode=mode, limit=limit)

    # 2. a male name, female name, or surname on its own
    bad_female_names = get_bad_passwords_from_iterable(
        female_names, mode=mode, limit=limit)
    bad_male_names = get_bad_passwords_from_iterable(
        male_names, mode=mode, limit=limit)
    bad_surnames = get_bad_passwords_from_iterable(
        surnames, mode=mode, limit=limit)

    return chain(bad_zxcvbn_passwords, bad_female_names, bad_male_names, bad_surnames)


print('Writing easy bad passwords: ', end='', flush=True)
with open('EASY_BAD_PASSWORDS.txt', 'wt') as fp:
    num = 0
    for pw in get_easy_bad_passwords():
        # print (pw)
        if num % 1000 == 0:
            print('.', end='', flush=True)
        fp.write(pw + '\n')
        num += 1

print('')
print('Number of easy passwords: {}'.format(num))

#################################
## MEDIUM (BAD) PASSWORDS      ##
#################################


def get_medium_bad_passwords_base():
    # 1. has all the easy passwords
    easy_bad_passwords = get_easy_bad_passwords(mode='rand')

    # 2. a male name + a surname or a female name + surname (or the reverse)
    male_name_plus_surname = combine_two_lists_into_bad_passwords(
        male_names, surnames)
    female_name_plus_surname = combine_two_lists_into_bad_passwords(
        female_names, surnames)

    # 3. anywhere from 2 to 4 english words
    two_words = get_bad_passwords_from_iterable(
        generate_permutations(english_words, 2, amount=1e3))
    three_words = get_bad_passwords_from_iterable(
        generate_permutations(english_words, 3, amount=1e3))
    four_words = get_bad_passwords_from_iterable(
        generate_permutations(english_words, 4, amount=1e3))

    return chain(easy_bad_passwords, male_name_plus_surname, female_name_plus_surname,
                 two_words, three_words, four_words)


def get_medium_bad_passwords():
    # 4. potential l33tings of these passwords
    return chain(get_medium_bad_passwords_base(), all_bad_l33ts(get_medium_bad_passwords_base()))


print('Writing medium bad passwords: ', end='', flush=True)
with open('MEDIUM_BAD_PASSWORDS.txt', 'wt') as fp:
    num = 0
    for pw in get_medium_bad_passwords():
        # print(pw)
        if num % 1000 == 0:
            print('.', end='', flush=True)
        fp.write(pw + '\n')
        num += 1

print('')
print('Number of medium passwords: {}'.format(num))


#################################
## HARD (BAD) PASSWORDS        ##
#################################

def get_hard_bad_passwords_base():
    # 1. Has all the medium passwords
    medium_bad_passwords = get_medium_bad_passwords_base()

    return medium_bad_passwords


def get_hard_bad_passwords():
    hard_base = get_hard_bad_passwords_base()

    # 2. All the passwords reversed
    hard_reversed = get_bad_reversed(get_hard_bad_passwords_base())

    # 3. All the passwords, but add punctuation to the end
    hard_punctuation = append_punctuation(get_hard_bad_passwords_base())

    # 4. All the passwords, but add number(s) to the end
    hard_numbers = append_numbers(get_hard_bad_passwords_base())

    # 5. All of the above, but l33t it
    hard_l33t = all_bad_l33ts(get_hard_bad_passwords_base())
    hard_reversed_l33t = all_bad_l33ts(
        get_bad_reversed(get_hard_bad_passwords_base()))
    hard_punctuation_l33t = append_punctuation(
        all_bad_l33ts(get_hard_bad_passwords_base()))
    hard_numbers_l33t = append_numbers(
        all_bad_l33ts(get_hard_bad_passwords_base()))

    return chain(hard_base, hard_reversed, hard_punctuation, hard_numbers, hard_l33t,
                 hard_reversed_l33t, hard_punctuation_l33t, hard_numbers_l33t)


print('Writing hard bad passwords: ', end='', flush=True)
with open('HARD_BAD_PASSWORDS.txt', 'wt') as fp:
    num = 0
    for pw in get_hard_bad_passwords():
        if num % 1000 == 0:
            print('.', end='', flush=True)
        fp.write(pw + '\n')
        num += 1

print('')
print('Number of hard passwords: {}'.format(num))
