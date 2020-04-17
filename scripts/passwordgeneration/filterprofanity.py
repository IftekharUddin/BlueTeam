# this file is a simple profanity filter which uses a dictionary
# you can also look into profanity filter modules (like profanity-filter or profanity-check, but this works as a simple
# band-aid)

# this profanity list was found at https://www.cs.cmu.edu/~biglou/resources/bad-words.txt
profane_file = 'profane.txt'
with open(profane_file, 'rt') as fp:
    profane_words = fp.read()

profane_words = profane_words.split('\n')


def is_profanity(test):
    for word in profane_words:
        if word in test:
            return True
    return False


files = ['english', 'female_names', 'male_names',
         'passwords', 'surnames', 'us_tv_and_film']


def filter_profanity_in_file(file, prefix='', extension='.txt'):
    with open(f'{prefix}{file}{extension}', 'rt') as fp:
        words = fp.read().split('\n')

    original_len = len(words)
    clean_words = [word for word in words if not is_profanity(word)]
    new_len = len(clean_words)
    # print ('{}, {}'.format(original_len, new_len))

    print('Number of profane words in file {}: {}'.format(
        file, original_len - new_len))

    with open(f'{prefix}{file}{extension}', 'wt') as fp:
        fp.write('\n'.join(clean_words))


for f in files:
    filter_profanity_in_file(
        f, prefix='../../PasswordDictionaries/', extension='.lst')
